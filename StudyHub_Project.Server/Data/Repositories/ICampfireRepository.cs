using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public interface ICampfireRepository
{
    Campfire? GetByUserId(
        int userId);

    Campfire Save(
        Campfire campfire);

    void Delete(
        int userId);
}