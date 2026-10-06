-- Справочник уровней сложности.
create table difficulty_levels(
    id     smallint primary key,
    name   text not null unique,
    points smallint not null,
    logs   smallint not null
);

insert into difficulty_levels (id, name, points, logs) values
    (1, 'Лёгкая',  10,  5),
    (2, 'Средняя', 20, 10),
    (3, 'Сложная', 30, 15);
