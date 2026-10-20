-- Запросы задач.


-- Справочники

-- GetDifficultyLevels: уровни сложности.
select id, name, points, logs
from difficulty_levels
order by id;

-- GetTaskStatuses: статусы задач.
select id, name
from task_statuses
order by id;


-- Списки

-- GetTasksByDay: задачи на день.
select id, title, description, date, difficulty_id, status_id
from tasks
where user_id = @UserId
  and date = @Date
order by created_at;

-- GetTasks: все задачи пользователя.
select id, title, description, date, difficulty_id, status_id, completed_at
from tasks
where user_id = @UserId;


-- Изменения

-- CreateTask: новая задача.
insert into tasks (user_id, title, description, date, difficulty_id)
values (@UserId, @Title, @Description, @Date, @DifficultyId)
returning id;

-- UpdateTask: изменение активной задачи.
update tasks
set title         = @Title,
    description   = @Description,
    date          = @Date,
    difficulty_id = @DifficultyId
where id = @TaskId
  and user_id = @UserId
  and status_id = 1;

-- DeleteTask: удаление активной задачи.
delete from tasks
where id = @TaskId
  and user_id = @UserId
  and status_id = 1;

-- SetTaskStatus: отметка «Выполнена» (2) или «Не выполнена» (3).
-- При выполнении — в одной транзакции с UpdateFire (fire.sql).
update tasks
set status_id = @StatusId,
    completed_at = now()
where id = @TaskId
  and user_id = @UserId
  and status_id = 1
returning difficulty_id;

-- GetTaskStatus: статус задачи, если изменение не сработало.
select status_id
from tasks
where id = @TaskId
  and user_id = @UserId;
