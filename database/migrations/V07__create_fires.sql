-- Костёр пользователя.
create table fires(
    id              bigserial primary key,
    user_id         uuid not null unique references users on delete cascade,
    logs            smallint not null default 0,
    logs_updated_at timestamptz not null default now()
);
