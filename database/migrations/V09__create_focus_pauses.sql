-- Паузы сессии фокуса.
create table focus_pauses(
    id         bigserial primary key,
    session_id bigint not null references focus_sessions on delete cascade,
    started_at timestamptz not null,
    ended_at   timestamptz
);

-- Все паузы сессии: подсчёт времени фокуса
create index focus_pauses_session_idx on focus_pauses (session_id);

-- У сессии может быть только одна незакрытая пауза
create unique index focus_pauses_one_open_idx
    on focus_pauses (session_id) where ended_at is null;
