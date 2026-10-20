using StudyHub_Project.Server.Data;
using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.DTOs.Focus;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Services;

public class FocusService
{
    private const int MinSeconds = 60;

    private const int MaxSeconds =
        12 * 60 * 60;


    private readonly IFocusSessionRepository
        _focusRepository;

    private readonly InMemoryDataStore
        _dataStore;


    public FocusService(
        IFocusSessionRepository focusRepository,
        InMemoryDataStore dataStore)
    {
        _focusRepository =
            focusRepository;

        _dataStore =
            dataStore;
    }


    public IReadOnlyCollection<FocusSession> GetAll(
        int userId)
    {
        return _focusRepository.GetAll(
            userId);
    }


    public FocusSession Start(
        int userId,
        StartFocusRequest request)
    {
        if (request.PlannedSeconds <
                MinSeconds ||
            request.PlannedSeconds >
                MaxSeconds)
        {
            throw new ApiException(
                400,
                "Укажите время от 1 минуты до 12 часов");
        }


        lock (_dataStore.FocusSyncRoot)
        {
            var hasActive =
                _focusRepository
                    .GetAll(userId)
                    .Any(session =>
                        session.Status ==
                        FocusSessionStatus.Active);


            if (hasActive)
            {
                throw new ApiException(
                    409,
                    "Сначала остановите текущий таймер");
            }


            var session =
                new FocusSession
                {
                    UserId = userId,

                    PlannedSeconds =
                        request.PlannedSeconds,

                    FocusedSeconds = 0,

                    Status =
                        FocusSessionStatus.Active,

                    StartedAt =
                        DateTimeOffset.UtcNow,

                    EndedAt =
                        null
                };


            return _focusRepository.Add(
                session);
        }
    }


    public FocusSession Finish(
        int userId,
        int id,
        FinishFocusRequest request)
    {
        lock (_dataStore.FocusSyncRoot)
        {
            var session =
                _focusRepository.GetById(
                    userId,
                    id);


            if (session is null)
            {
                throw new ApiException(
                    404,
                    "Сессия не найдена");
            }


            if (request.Status ==
                FocusSessionStatus.Active)
            {
                throw new ApiException(
                    400,
                    "Некорректный статус завершения сессии");
            }


            if (session.Status !=
                FocusSessionStatus.Active)
            {
                return session;
            }


            session.Status =
                request.Status;


            session.FocusedSeconds =
                Math.Clamp(
                    request.FocusedSeconds,
                    0,
                    session.PlannedSeconds);


            session.EndedAt =
                DateTimeOffset.UtcNow;


            return _focusRepository.Update(
                session);
        }
    }
}