-- Пользователи.
create table users(
    id            uuid primary key default uuidv7(),
    name          varchar(50) not null,
    email         text not null unique,
    phone         varchar(11) not null unique,
    password_hash text not null,
    created_at    timestamptz not null default now()
);

-- e-mail нельзя зарегистрировать повторно в другом регистре
create unique index users_email_lower_idx on users (lower(email));
