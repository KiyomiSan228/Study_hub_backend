using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public interface IUserRepository
{
    UserAccount? GetById(int id);

    UserAccount? GetByEmail(string email);

    UserAccount? GetByPhone(string phone);

    bool EmailExists(
        string email,
        int? exceptUserId = null);

    bool PhoneExists(
        string phone,
        int? exceptUserId = null);

    UserAccount Add(UserAccount user);

    UserAccount Update(UserAccount user);

    bool Delete(int id);
}