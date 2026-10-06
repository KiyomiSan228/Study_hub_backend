-- Записи мини-блокнота. 
create table notes(
    id         serial primary key,
    user_id    integer not null references users,
    title      varchar(200) not null,
    body       text not null,
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);
