-- Паузы внутри фокус-сессии.
create table focus_pauses(
    id         serial primary key,
    session_id integer not null references focus_sessions,
    started_at timestamptz not null,
    ended_at   timestamptz                       
);
