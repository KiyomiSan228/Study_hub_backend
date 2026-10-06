-- Сессии таймера фокуса.
create table focus_sessions(
    id              serial primary key,
    user_id         integer not null references users,
    planned_seconds integer not null,           
    started_at      timestamptz not null,
    ended_at        timestamptz not null,
    status          text not null             
);
