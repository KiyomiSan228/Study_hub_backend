-- Костёр пользователя
create table fires(
    id              serial primary key,
    user_id         integer not null unique references users,
    logs            smallint not null default 0,          
    logs_updated_at timestamptz not null default now()      
);
