-- Попытки входа.
create table login_attempts(
    id           bigserial primary key,
    user_id      uuid references users on delete cascade,
    login        text not null,
    success      boolean not null,
    attempted_at timestamptz not null default now()
);

-- Блокировка входа: последние попытки по введённому логину
create index login_attempts_login_time_idx on login_attempts (login, attempted_at);

-- Удаление аккаунта: быстро найти попытки входа пользователя
create index login_attempts_user_idx on login_attempts (user_id);
