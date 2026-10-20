using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Data.Repositories;

public class InMemoryNoteRepository : INoteRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryNoteRepository(
        InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyCollection<Note> GetAll(
        int userId)
    {
        return _dataStore.Notes
            .Values
            .Where(note =>
                note.UserId == userId)
            .OrderByDescending(note =>
                note.IsPinned)
            .ThenByDescending(note =>
                note.UpdatedAt)
            .ToList();
    }

    public Note? GetById(
        int userId,
        int id)
    {
        var found =
            _dataStore.Notes.TryGetValue(
                id,
                out var note);


        if (!found || note is null)
        {
            return null;
        }


        if (note.UserId != userId)
        {
            return null;
        }


        return note;
    }

    public Note Add(
        Note note)
    {
        if (note.UserId <= 0)
        {
            throw new InvalidOperationException(
                "У заметки не указан пользователь.");
        }

        note.Id =
            _dataStore.GetNextNoteId();



        var added =
            _dataStore.Notes.TryAdd(
                note.Id,
                note);


        if (!added)
        {
            throw new InvalidOperationException(
                "Не удалось сохранить заметку.");
        }


        return note;
    }

    public Note Update(
        Note note)
    {
        var found =
            _dataStore.Notes.TryGetValue(
                note.Id,
                out var existingNote);


        if (!found || existingNote is null)
        {
            throw new KeyNotFoundException(
                "Заметка не найдена.");
        }


        if (existingNote.UserId != note.UserId)
        {
            throw new InvalidOperationException(
                "Нельзя изменить чужую заметку.");
        }


        _dataStore.Notes[note.Id] =
            note;


        return note;
    }

    public bool Delete(
        int userId,
        int id)
    {
        var found =
            _dataStore.Notes.TryGetValue(
                id,
                out var note);


        if (!found || note is null)
        {
            return false;
        }

        if (note.UserId != userId)
        {
            return false;
        }


        return _dataStore.Notes.TryRemove(
            id,
            out _);
    }

    public void DeleteAllByUserId(
        int userId)
    {

        var noteIds =
            _dataStore.Notes
                .Values
                .Where(note =>
                    note.UserId == userId)
                .Select(note =>
                    note.Id)
                .ToList();

        foreach (var noteId in noteIds)
        {
            _dataStore.Notes.TryRemove(
                noteId,
                out _);
        }
    }
}