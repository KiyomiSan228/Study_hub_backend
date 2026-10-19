-- Сессии таймера фокуса.
create table focus_sessions(
    id              bigserial primary key,
    user_id         uuid not null references users on delete cascade,
    planned_seconds integer not null,
    started_at      timestamptz not null,
    ended_at        timestamptz,
    status_id       smallint not null default 1 references focus_statuses
);

-- Статистика: время фокуса пользователя за период
create index focus_sessions_user_started_idx on focus_sessions (user_id, started_at);

-- У пользователя может быть только одна активная сессия фокуса
create unique index focus_sessions_one_active_idx
    on focus_sessions (user_id) where status_id = 1;
