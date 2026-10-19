-- Записи блокнота.
create table notes(
    id         bigserial primary key,
    user_id    uuid not null references users on delete cascade,
    title      varchar(200) not null,
    body       text not null,
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);

-- Записи пользователя по дате изменения
create index notes_user_updated_idx on notes (user_id, updated_at);
