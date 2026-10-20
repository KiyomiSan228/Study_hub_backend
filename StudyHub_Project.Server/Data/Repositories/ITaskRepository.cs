using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories
{
    public interface ITaskRepository
    {
        IReadOnlyCollection<StudyTask> GetAll(int userId);
        IReadOnlyCollection<StudyTask> GetByDate(int userId, DateOnly date);
        StudyTask? GetById(int userId, int id);
        StudyTask Add(StudyTask task);
        StudyTask Update(StudyTask task);
        bool Delete(int userId, int id);
        void DeleteAllByUserId(int userId);
    }
}

