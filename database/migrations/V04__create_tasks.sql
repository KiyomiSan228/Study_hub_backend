-- Задачи пользователя.
create table tasks(
    id            bigserial primary key,
    user_id       uuid not null references users on delete cascade,
    difficulty_id smallint not null references difficulty_levels,
    status_id     smallint not null default 1 references task_statuses,
    title         varchar(200) not null,
    description   text,
    date          date not null,
    created_at    timestamptz not null default now(),
    completed_at  timestamptz
);

-- Главный экран: задачи пользователя по дате
create index tasks_user_date_idx on tasks (user_id, date);

-- Статистика: выполненные задачи пользователя за период
create index tasks_user_completed_idx on tasks (user_id, completed_at);
