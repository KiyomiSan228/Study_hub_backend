using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public interface IFocusSessionRepository
{
    IReadOnlyCollection<FocusSession> GetAll(
        int userId);

    FocusSession? GetById(
        int userId,
        int id);

    FocusSession Add(
        FocusSession session);

    FocusSession Update(
        FocusSession session);

    void DeleteAllByUserId(
        int userId);
}