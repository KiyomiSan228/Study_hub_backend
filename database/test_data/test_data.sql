-- Тестовые данные для разработки. Выполнять вручную после всех скриптов из migrations.
insert into users (name, email, phone, password_hash) values
    ('Демо-пользователь', 'demo@studyhub.local', '79990000000', 'demo-no-login'),
    ('Анна Иванова',      'anna@studyhub.local', '79991112233', 'test-hash-1'),
    ('Пётр Смирнов',      'petr@studyhub.local', '79994445566', 'test-hash-2');

-- У каждого пользователя свой костёр (пока потухший).
insert into fires (user_id) select id from users;
