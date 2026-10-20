-- Запросы блокнота.

-- GetNotes: записи от новых к старым.
select id, title, body, updated_at
from notes
where user_id = @UserId
order by updated_at desc;

-- CreateNote: новая запись.
insert into notes (user_id, title, body)
values (@UserId, @Title, @Body)
returning id;

-- UpdateNote: изменение записи.
update notes
set title = @Title,
    body = @Body,
    updated_at = now()
where id = @NoteId
  and user_id = @UserId;

-- DeleteNote: удаление записи.
delete from notes
where id = @NoteId
  and user_id = @UserId;
