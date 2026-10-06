-- Задачи пользователя.
create table tasks(
    id            serial primary key,
    user_id       integer not null references users,
    difficulty_id smallint not null references difficulty_levels,
    status_id     smallint not null default 1 references task_statuses, 
    title         varchar(200) not null,
    description   text,
    date          date not null, 
    created_at    timestamptz not null default now(),
    completed_at  timestamptz           
);
