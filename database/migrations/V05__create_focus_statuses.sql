-- Статусы сессии фокуса.
create table focus_statuses(
    id   smallint primary key,
    name text not null unique
);

insert into focus_statuses (id, name) values
    (1, 'Активна'),
    (2, 'Завершена'),
    (3, 'Прервана');
