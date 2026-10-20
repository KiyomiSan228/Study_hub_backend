using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public class InMemoryFocusSessionRepository : IFocusSessionRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryFocusSessionRepository(
        InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyCollection<FocusSession> GetAll(
        int userId)
    {
        return _dataStore.FocusSessions
            .Values
            .Where(session =>
                session.UserId == userId)
            .OrderByDescending(session =>
                session.StartedAt)
            .ToList();
    }

    public FocusSession? GetById(
        int userId,
        int id)
    {
        var found =
            _dataStore.FocusSessions.TryGetValue(
                id,
                out var session);


        if (!found || session is null)
        {
            return null;
        }


        if (session.UserId != userId)
        {
            return null;
        }


        return session;
    }

    public FocusSession Add(
        FocusSession session)
    {

        if (session.UserId <= 0)
        {
            throw new InvalidOperationException(
                "У Focus-сессии не указан пользователь.");
        }

        session.Id =
            _dataStore.GetNextFocusSessionId();

        var added =
            _dataStore.FocusSessions.TryAdd(
                session.Id,
                session);


        if (!added)
        {
            throw new InvalidOperationException(
                "Не удалось сохранить Focus-сессию.");
        }


        return session;
    }

    public FocusSession Update(
        FocusSession session)
    {
        var found =
            _dataStore.FocusSessions.TryGetValue(
                session.Id,
                out var existingSession);


        if (!found || existingSession is null)
        {
            throw new KeyNotFoundException(
                "Focus-сессия не найдена.");
        }


        if (existingSession.UserId != session.UserId)
        {
            throw new InvalidOperationException(
                "Нельзя изменить чужую Focus-сессию.");
        }


        _dataStore.FocusSessions[session.Id] =
            session;


        return session;
    }

    public void DeleteAllByUserId(
        int userId)
    {
        var sessionIds =
            _dataStore.FocusSessions
                .Values
                .Where(session =>
                    session.UserId == userId)
                .Select(session =>
                    session.Id)
                .ToList();

        foreach (var sessionId in sessionIds)
        {
            _dataStore.FocusSessions.TryRemove(
                sessionId,
                out _);
        }
    }
}