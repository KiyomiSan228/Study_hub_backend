-- Сеансы входа. 
create table auth_sessions(
    id         serial primary key,
    user_id    integer not null references users,
    token_hash text not null unique,
    created_at timestamptz not null default now(),
    expires_at timestamptz not null,
    revoked_at timestamptz
);
