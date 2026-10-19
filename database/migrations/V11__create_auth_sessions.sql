-- Сеансы входа.
create table auth_sessions(
    id         bigserial primary key,
    user_id    uuid not null references users on delete cascade,
    token_hash text not null unique,
    created_at timestamptz not null default now(),
    expires_at timestamptz not null,
    revoked_at timestamptz
);

-- Поиск всех сеансов пользователя
create index auth_sessions_user_idx on auth_sessions (user_id);