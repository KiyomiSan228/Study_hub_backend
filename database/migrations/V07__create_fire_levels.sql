-- Справочник состояний костра. Одинаков для всех пользователей.
create table fire_levels(
    id       smallint primary key,
    name     varchar(20) not null unique,
    min_logs smallint not null unique
);

insert into fire_levels (id, name, min_logs) values
    (1, 'Потух',   0),
    (2, 'Тлеет',   1),
    (3, 'Горит',  30),
    (4, 'Пылает', 70);
