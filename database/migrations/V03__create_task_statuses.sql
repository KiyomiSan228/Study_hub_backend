-- Справочник статусов задачи.
create table task_statuses(
    id   smallint primary key,
    name text not null unique
);

insert into task_statuses (id, name) values
    (1, 'Активна'),
    (2, 'Выполнена'),
    (3, 'Не выполнена');
