using StudyHub_Project.Server.Data;
using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.DTOs.Tasks;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Services;

public class TaskService
{
    private const int TitleMaxLength = 200;


    private readonly ITaskRepository _taskRepository;

    private readonly CampfireService _campfireService;

    private readonly InMemoryDataStore _dataStore;


    public TaskService(
        ITaskRepository taskRepository,
        CampfireService campfireService,
        InMemoryDataStore dataStore)
    {
        _taskRepository = taskRepository;
        _campfireService = campfireService;
        _dataStore = dataStore;
    }


    public IReadOnlyCollection<StudyTask> GetAll(
        int userId)
    {
        return _taskRepository.GetAll(
            userId);
    }


    public IReadOnlyCollection<StudyTask> GetByDate(
        int userId,
        DateOnly date)
    {
        return _taskRepository.GetByDate(
            userId,
            date);
    }


    public StudyTask Create(
        int userId,
        CreateTaskRequest request)
    {
        var title =
            ValidateTitle(
                request.Title);


        if (request.Date <
            GetToday())
        {
            throw new ApiException(
                400,
                "Нельзя добавить задачу на прошедший день");
        }


        var task =
            new StudyTask
            {
                UserId = userId,
                Title = title,

                Description =
                    NormalizeOptionalText(
                        request.Description),

                Date = request.Date,

                Difficulty =
                    request.Difficulty,

                Category =
                    NormalizeOptionalText(
                        request.Category),

                Status =
                    StudyTaskStatus.Active,

                CreatedAt =
                    DateTimeOffset.UtcNow,

                CompletedAt =
                    null
            };


        return _taskRepository.Add(
            task);
    }


    public StudyTask Update(
        int userId,
        int id,
        UpdateTaskRequest request)
    {
        lock (_dataStore.TaskCampfireSyncRoot)
        {
            var task =
                GetRequiredTask(
                    userId,
                    id);


            EnsureActive(
                task,
                "Отмеченную задачу нельзя изменить");


            var title =
                ValidateTitle(
                    request.Title);


            if (request.Date != task.Date &&
                request.Date < GetToday())
            {
                throw new ApiException(
                    400,
                    "Нельзя перенести задачу на прошедший день");
            }


            task.Title = title;

            task.Description =
                NormalizeOptionalText(
                    request.Description);

            task.Date =
                request.Date;

            task.Difficulty =
                request.Difficulty;

            task.Category =
                NormalizeOptionalText(
                    request.Category);


            return _taskRepository.Update(
                task);
        }
    }


    public void Delete(
        int userId,
        int id)
    {
        lock (_dataStore.TaskCampfireSyncRoot)
        {
            var task =
                GetRequiredTask(
                    userId,
                    id);


            EnsureActive(
                task,
                "Отмеченную задачу удалить нельзя");


            if (!_taskRepository.Delete(
                    userId,
                    id))
            {
                throw new ApiException(
                    404,
                    "Задача уже удалена");
            }
        }
    }


    public CompleteTaskResponse Complete(
        int userId,
        int id)
    {
        lock (_dataStore.TaskCampfireSyncRoot)
        {
            var task =
                GetRequiredTask(
                    userId,
                    id);


            EnsureActive(
                task,
                "Задача уже отмечена, изменить её нельзя");


            var now =
                DateTimeOffset.UtcNow;


            task.Status =
                StudyTaskStatus.Completed;

            task.CompletedAt =
                now;


            _taskRepository.Update(
                task);


            var logs =
                task.Difficulty switch
                {
                    TaskDifficulty.Easy => 5,
                    TaskDifficulty.Medium => 10,
                    TaskDifficulty.Hard => 15,
                    _ => 0
                };


            var campfire =
                _campfireService.AddLogs(
                    userId,
                    logs,
                    now);


            return new CompleteTaskResponse
            {
                Task = task,
                Campfire = campfire.Campfire,
                LogsAdded = campfire.Added
            };
        }
    }


    public StudyTask Fail(
        int userId,
        int id)
    {
        lock (_dataStore.TaskCampfireSyncRoot)
        {
            var task =
                GetRequiredTask(
                    userId,
                    id);


            EnsureActive(
                task,
                "Задача уже отмечена, изменить её нельзя");


            task.Status =
                StudyTaskStatus.Failed;

            task.CompletedAt =
                DateTimeOffset.UtcNow;


            return _taskRepository.Update(
                task);
        }
    }


    private StudyTask GetRequiredTask(
        int userId,
        int id)
    {
        var task =
            _taskRepository.GetById(
                userId,
                id);


        if (task is null)
        {
            throw new ApiException(
                404,
                "Задача не найдена или удалена");
        }


        return task;
    }


    private static void EnsureActive(
        StudyTask task,
        string message)
    {
        if (task.Status !=
            StudyTaskStatus.Active)
        {
            throw new ApiException(
                409,
                message);
        }
    }


    private static string ValidateTitle(
        string title)
    {
        var normalized =
            title.Trim();


        if (string.IsNullOrWhiteSpace(
                normalized))
        {
            throw new ApiException(
                400,
                "Введите название задачи");
        }


        if (normalized.Length >
            TitleMaxLength)
        {
            throw new ApiException(
                400,
                $"Название задачи не должно быть длиннее {TitleMaxLength} символов");
        }


        return normalized;
    }


    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }


    private static DateOnly GetToday()
    {
        return DateOnly.FromDateTime(
            DateTime.Now);
    }
}