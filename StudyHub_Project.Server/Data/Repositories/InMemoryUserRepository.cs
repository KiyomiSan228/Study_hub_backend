using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryDataStore _dataStore;


    public InMemoryUserRepository(
        InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public UserAccount? GetById(int id)
    {
        var found = _dataStore.Users.TryGetValue(
            id,
            out var user);

        return found
            ? user
            : null;
    }

    public UserAccount? GetByEmail(string email)
    {
        return _dataStore.Users
            .Values
            .FirstOrDefault(user =>
                string.Equals(
                    user.Email,
                    email,
                    StringComparison.OrdinalIgnoreCase));
    }

    public UserAccount? GetByPhone(string phone)
    {
        return _dataStore.Users
            .Values
            .FirstOrDefault(user =>
                user.Phone == phone);
    }

    public bool EmailExists(
        string email,
        int? exceptUserId = null)
    {
        return _dataStore.Users
            .Values
            .Any(user =>
                user.Id != exceptUserId &&
                string.Equals(
                    user.Email,
                    email,
                    StringComparison.OrdinalIgnoreCase));
    }


    public bool PhoneExists(
        string phone,
        int? exceptUserId = null)
    {
        return _dataStore.Users
            .Values
            .Any(user =>
                user.Id != exceptUserId &&
                user.Phone == phone);
    }

    public UserAccount Add(UserAccount user)
    {
        user.Id =
            _dataStore.GetNextUserId();


        var added =
            _dataStore.Users.TryAdd(
                user.Id,
                user);


        if (!added)
        {
            throw new InvalidOperationException(
                "Не удалось сохранить пользователя.");
        }


        return user;
    }

    public UserAccount Update(UserAccount user)
    {

        if (!_dataStore.Users.ContainsKey(
                user.Id))
        {
            throw new KeyNotFoundException(
                "Пользователь не найден.");
        }

        _dataStore.Users[user.Id] =
            user;


        return user;
    }

    public bool Delete(int id)
    {
        return _dataStore.Users.TryRemove(
            id,
            out _);
    }
}