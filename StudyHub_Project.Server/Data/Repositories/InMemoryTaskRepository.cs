using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTaskRepository(
        InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyCollection<StudyTask> GetAll(
        int userId)
    {
        return _dataStore.Tasks
            .Values
            .Where(task =>
                task.UserId == userId)
            .OrderBy(task =>
                task.Date)
            .ThenByDescending(task =>
                task.CreatedAt)
            .ToList();
    }

    public IReadOnlyCollection<StudyTask> GetByDate(
        int userId,
        DateOnly date)
    {
        return _dataStore.Tasks
            .Values
            .Where(task =>
                task.UserId == userId &&
                task.Date == date)
            .OrderByDescending(task =>
                task.CreatedAt)
            .ToList();
    }

    public StudyTask? GetById(
        int userId,
        int id)
    {
        var found =
            _dataStore.Tasks.TryGetValue(
                id,
                out var task);


        if (!found || task is null)
        {
            return null;
        }


        if (task.UserId != userId)
        {
            return null;
        }


        return task;
    }

    public StudyTask Add(
        StudyTask task)
    {

        if (task.UserId <= 0)
        {
            throw new InvalidOperationException(
                "У задачи не указан пользователь.");
        }

        task.Id =
            _dataStore.GetNextTaskId();

        var added =
            _dataStore.Tasks.TryAdd(
                task.Id,
                task);


        if (!added)
        {
            throw new InvalidOperationException(
                "Не удалось сохранить задачу.");
        }


        return task;
    }
    public StudyTask Update(
        StudyTask task)
    {
        var found =
            _dataStore.Tasks.TryGetValue(
                task.Id,
                out var existingTask);


        if (!found || existingTask is null)
        {
            throw new KeyNotFoundException(
                "Задача не найдена.");
        }

        if (existingTask.UserId != task.UserId)
        {
            throw new InvalidOperationException(
                "Нельзя изменить чужую задачу.");
        }


        _dataStore.Tasks[task.Id] =
            task;


        return task;
    }

    public bool Delete(
        int userId,
        int id)
    {
        var found =
            _dataStore.Tasks.TryGetValue(
                id,
                out var task);


        if (!found || task is null)
        {
            return false;
        }

        if (task.UserId != userId)
        {
            return false;
        }


        return _dataStore.Tasks.TryRemove(
            id,
            out _);
    }

    public void DeleteAllByUserId(
        int userId)
    {
        var tasksToDelete =
            _dataStore.Tasks
                .Values
                .Where(task =>
                    task.UserId == userId)
                .Select(task =>
                    task.Id)
                .ToList();


        foreach (var taskId in tasksToDelete)
        {
            _dataStore.Tasks.TryRemove(
                taskId,
                out _);
        }
    }
}