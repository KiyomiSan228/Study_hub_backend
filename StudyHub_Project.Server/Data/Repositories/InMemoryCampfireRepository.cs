using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public class InMemoryCampfireRepository : ICampfireRepository
{
    private readonly InMemoryDataStore _dataStore;


    public InMemoryCampfireRepository(
        InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Campfire? GetByUserId(
        int userId)
    {
        var found =
            _dataStore.Campfires.TryGetValue(
                userId,
                out var campfire);


        return found
            ? campfire
            : null;
    }

    public Campfire Save(
        Campfire campfire)
    {
        if (campfire.UserId <= 0)
        {
            throw new InvalidOperationException(
                "У костра не указан пользователь.");
        }
        if (campfire.Logs < 0)
        {
            throw new InvalidOperationException(
                "Количество брёвен не может быть отрицательным.");
        }
        if (campfire.Logs > 100)
        {
            throw new InvalidOperationException(
                "Количество брёвен не может быть больше 100.");
        }
        _dataStore.Campfires[campfire.UserId] =
            campfire;

        return campfire;
    }

    public void Delete(
        int userId)
    {
        _dataStore.Campfires.TryRemove(
            userId,
            out _);
    }
}