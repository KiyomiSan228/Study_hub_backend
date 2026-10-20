-- Запросы костра.

-- GetFire: запас брёвен и время его изменения.
select logs, logs_updated_at
from fires
where user_id = @UserId;

-- GetFireLevels: состояния костра.
select id, name, min_logs
from fire_levels
order by min_logs;

-- UpdateFire: новый запас брёвен.
update fires
set logs = @Logs,
    logs_updated_at = @LogsUpdatedAt
where user_id = @UserId;
