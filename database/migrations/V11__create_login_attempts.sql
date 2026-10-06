-- Попытки входа.
create table login_attempts(
    id           serial primary key,
    user_id      integer references users,       
    login        text not null,                 
    success      boolean not null,
    attempted_at timestamptz not null default now()
);
