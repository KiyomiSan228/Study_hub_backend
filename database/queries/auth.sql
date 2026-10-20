-- Запросы аккаунта.


-- Регистрация
-- В одной транзакции: RegisterUser, CreateFire, CreateAuthSession.

-- RegisterUser: новый пользователь.
insert into users (name, email, phone, password_hash)
values (@Name, @Email, @Phone, @PasswordHash)
returning id;

-- CreateFire: костёр нового пользователя.
insert into fires (user_id)
values (@UserId);

-- CreateAuthSession: сеанс входа.
insert into auth_sessions (user_id, token_hash, expires_at)
values (@UserId, @TokenHash, now() + interval '30 days');


-- Вход

-- GetLastLoginAttempts: последние 5 попыток входа по логину.
select success, attempted_at
from login_attempts
where login = @Login
order by attempted_at desc
limit 5;

-- FindUserByEmail: пользователь по e-mail.
select id, password_hash
from users
where lower(email) = lower(@Email);

-- FindUserByPhone: пользователь по номеру.
select id, password_hash
from users
where phone = @Phone;

-- AddLoginAttempt: запись попытки входа.
insert into login_attempts (user_id, login, success)
values (@UserId, @Login, @Success);


-- Сеанс

-- GetUserIdByToken: пользователь по токену.
select user_id
from auth_sessions
where token_hash = @TokenHash
  and revoked_at is null
  and expires_at > now();

-- Logout: выход.
update auth_sessions
set revoked_at = now()
where token_hash = @TokenHash;


-- Профиль

-- GetAccount: данные пользователя.
select id, name, email, phone, created_at
from users
where id = @UserId;

-- GetPasswordHash: хэш пароля для проверки.
select password_hash
from users
where id = @UserId;

-- UpdateAccount: имя, e-mail и номер.
update users
set name  = @Name,
    email = @Email,
    phone = @Phone
where id = @UserId;

-- UpdatePassword: новый пароль.
update users
set password_hash = @PasswordHash
where id = @UserId;

-- DeleteAccount: удаление аккаунта.
delete from users
where id = @UserId;
