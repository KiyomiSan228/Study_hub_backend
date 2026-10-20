-- Запросы таймера фокуса.


-- Сессии

-- GetFocusSessions: все сессии пользователя.
select id, planned_seconds, started_at, ended_at, status_id
from focus_sessions
where user_id = @UserId
order by started_at desc;

-- GetActiveFocusSession: активная сессия.
select id, planned_seconds, started_at
from focus_sessions
where user_id = @UserId
  and status_id = 1;

-- StartFocusSession: запуск таймера.
insert into focus_sessions (user_id, planned_seconds, started_at)
values (@UserId, @PlannedSeconds, now())
returning id;

-- EndFocusSession: «Завершена» (2) или «Прервана» (3).
update focus_sessions
set status_id = @StatusId,
    ended_at = @EndedAt
where id = @SessionId
  and user_id = @UserId
  and status_id = 1;


-- Паузы
-- @SessionId — id из GetActiveFocusSession, а не из запроса клиента.

-- GetSessionPauses: паузы сессии.
select started_at, ended_at
from focus_pauses
where session_id = @SessionId;

-- StartPause: пауза.
insert into focus_pauses (session_id, started_at)
values (@SessionId, now());

-- EndPause: продолжение после паузы (и перед EndFocusSession).
update focus_pauses
set ended_at = now()
where session_id = @SessionId
  and ended_at is null;
