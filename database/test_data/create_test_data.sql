-- Тестовые данные.
-- Даты считаются от сегодняшнего дня, поэтому данные выглядят свежими в любой день.

-- Пользователи (у всех пароль Demo12345, хранится bcrypt-хэш)
insert into users (name, email, phone, password_hash) values
    ('Демо-пользователь', 'demo@studyhub.local', '79990000000', '$2a$11$GyhNBrGacHSdCDJy8ailcuHgIGFQ5z7y/AQ0WQs/ZfmWOYq8HpLT2'),
    ('Анна Иванова',      'anna@studyhub.local', '79991112233', '$2a$11$GyhNBrGacHSdCDJy8ailcuHgIGFQ5z7y/AQ0WQs/ZfmWOYq8HpLT2'),
    ('Пётр Смирнов',      'petr@studyhub.local', '79994445566', '$2a$11$GyhNBrGacHSdCDJy8ailcuHgIGFQ5z7y/AQ0WQs/ZfmWOYq8HpLT2');

-- Костры: у демо-пользователя горит, у остальных потух
insert into fires (user_id, logs)
select id, case email when 'demo@studyhub.local' then 45 else 0 end
from users;

-- Задачи демо-пользователя: прошлая неделя, сегодня и будущие дни
insert into tasks (user_id, difficulty_id, status_id, title, description, date, completed_at)
select u.id, t.difficulty_id, t.status_id, t.title, t.description,
       current_date + t.day_offset,
       case when t.status_id = 1 then null
            else least((current_date + t.day_offset) + time '18:00', now()) end
from users u,
     (values
        ('Прочитать главу по ООП',          'Глава про наследование и полиморфизм',      2, 2, -6),
        ('Решить задачи по матанализу',     'Номера 12–20 из задачника',                 3, 2, -5),
        ('Сходить в библиотеку',            null,                                        1, 3, -5),
        ('Подготовить конспект по БД',      'Нормальные формы и индексы',                2, 2, -4),
        ('Сделать лабораторную по КГ',      'Освещение в OpenGL',                        3, 2, -3),
        ('Повторить английские слова',      null,                                        1, 2, -2),
        ('Написать эссе по этике ИИ',       'Тема: ответственность за решения ИИ',       3, 3, -2),
        ('Разобрать лекцию по электронике', 'Операционные усилители',                    2, 2, -1),
        ('Сделать ERD для проекта',         'Обновить диаграмму после перехода на UUID', 3, 2,  0),
        ('Ответить на письмо куратору',     null,                                        1, 2,  0),
        ('Подготовиться к защите лабы',     'Повторить',                                 2, 1,  0),
        ('Повторить SQL-запросы',           'join, group by, having',                    1, 1,  0),
        ('Сдать отчёт по ТКА',              null,                                        3, 1,  1),
        ('Купить тетради',                  null,                                        1, 1,  2),
        ('Начать курсовую',                 'Выбрать тему и составить план',             3, 1,  5)
     ) as t(title, description, difficulty_id, status_id, day_offset)
where u.email = 'demo@studyhub.local';

-- Пара задач у других пользователей
insert into tasks (user_id, difficulty_id, title, date)
select id, 2, 'Задача Анны на сегодня', current_date from users where email = 'anna@studyhub.local';

insert into tasks (user_id, difficulty_id, title, date)
select id, 1, 'Задача Петра на завтра', current_date + 1 from users where email = 'petr@studyhub.local';

-- Сессии фокуса демо-пользователя: 2 — завершена, 3 — прервана
insert into focus_sessions (user_id, planned_seconds, started_at, ended_at, status_id)
select u.id, s.planned,
       least((current_date + s.day_offset) + s.start_time, now() - s.duration),
       least((current_date + s.day_offset) + s.start_time, now() - s.duration) + s.duration,
       s.status_id
from users u,
     (values
        (1500, -5, time '10:00', interval '25 minutes', 2),
        (3000, -4, time '14:00', interval '50 minutes', 2),
        (1500, -3, time '11:00', interval '20 minutes', 3),
        (2700, -2, time '16:00', interval '45 minutes', 2),
        (1500, -1, time '09:30', interval '25 minutes', 2),
        (3600,  0, time '10:00', interval '40 minutes', 3)
     ) as s(planned, day_offset, start_time, duration, status_id)
where u.email = 'demo@studyhub.local';

-- Паузы в прерванных сессиях
insert into focus_pauses (session_id, started_at, ended_at)
select id, started_at + interval '10 minutes', started_at + interval '15 minutes'
from focus_sessions
where status_id = 3;

-- Записи блокнота демо-пользователя
insert into notes (user_id, title, body, created_at, updated_at)
select u.id, n.title, n.body, now() - n.age, now() - n.age
from users u,
     (values
        ('Идеи для курсовой', 'Трекер привычек.',          interval '5 days'),
        ('Вопросы к защите',  '??????',                    interval '1 day'),
        ('Список покупок',    'Тетради, ручки, маркеры.',  interval '2 hours')
     ) as n(title, body, age)
where u.email = 'demo@studyhub.local';
