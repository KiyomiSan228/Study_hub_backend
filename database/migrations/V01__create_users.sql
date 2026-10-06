-- Пользователи.
create table users(
    id            serial primary key,
    name          varchar(50) not null,
    email         text not null unique,
    phone         varchar(11) not null unique,  
    password_hash text not null,                
    created_at    timestamptz not null default now()
);
