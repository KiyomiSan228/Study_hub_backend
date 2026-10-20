using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Services;

public class CampfireService
{
    private const int MaxLogs = 100;


    private readonly ICampfireRepository
        _campfireRepository;


    public CampfireService(
        ICampfireRepository campfireRepository)
    {
        _campfireRepository =
            campfireRepository;
    }


    public Campfire Get(
        int userId)
    {
        var campfire =
            _campfireRepository.GetByUserId(
                userId);


        if (campfire is not null)
        {
            return campfire;
        }


        campfire =
            new Campfire
            {
                UserId = userId,
                Logs = 0,
                UpdatedAt =
                    DateTimeOffset.UtcNow
            };


        return _campfireRepository.Save(
            campfire);
    }


    public (
        Campfire Campfire,
        int Added)
        AddLogs(
            int userId,
            int amount,
            DateTimeOffset now)
    {
        if (amount < 0)
        {
            throw new ApiException(
                400,
                "Количество брёвен не может быть отрицательным");
        }


        var campfire =
            Get(userId);


        var elapsed =
            now - campfire.UpdatedAt;


        var burnedHours =
            elapsed <= TimeSpan.Zero
                ? 0
                : (int)Math.Floor(
                    elapsed.TotalHours);


        var currentLogs =
            Math.Max(
                0,
                campfire.Logs -
                burnedHours);


        var nextLogs =
            Math.Min(
                MaxLogs,
                currentLogs + amount);


        var actuallyAdded =
            nextLogs - currentLogs;


        DateTimeOffset updatedAt;


        if (currentLogs == 0)
        {
            updatedAt =
                now;
        }
        else
        {
            updatedAt =
                campfire.UpdatedAt
                    .AddHours(
                        burnedHours);
        }


        campfire.Logs =
            nextLogs;

        campfire.UpdatedAt =
            updatedAt;


        _campfireRepository.Save(
            campfire);


        return (
            campfire,
            actuallyAdded);
    }
}