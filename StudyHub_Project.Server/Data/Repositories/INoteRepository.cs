using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public interface INoteRepository
{
    IReadOnlyCollection<Note> GetAll(
        int userId);

    Note? GetById(
        int userId,
        int id);

    Note Add(Note note);

    Note Update(Note note);

    bool Delete(
        int userId,
        int id);

    void DeleteAllByUserId(
        int userId);
}