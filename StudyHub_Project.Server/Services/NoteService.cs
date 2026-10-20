using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.DTOs.Notes;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Services;

public class NoteService
{
    private const int TitleMaxLength = 200;
    private const int TextMaxLength = 5000;


    private readonly INoteRepository _noteRepository;


    public NoteService(
        INoteRepository noteRepository)
    {
        _noteRepository =
            noteRepository;
    }


    public IReadOnlyCollection<Note> GetAll(
        int userId)
    {
        return _noteRepository.GetAll(
            userId);
    }


    public Note Create(
        int userId,
        CreateNoteRequest request)
    {
        var now =
            DateTimeOffset.UtcNow;


        var note =
            new Note
            {
                UserId = userId,

                Title =
                    NormalizeTitle(
                        request.Title),

                Text =
                    NormalizeText(
                        request.Text),

                Category =
                    NormalizeOptionalText(
                        request.Category),

                IsPinned = false,

                CreatedAt = now,

                UpdatedAt = now
            };


        return _noteRepository.Add(
            note);
    }


    public Note Update(
        int userId,
        int id,
        UpdateNoteRequest request)
    {
        var note =
            _noteRepository.GetById(
                userId,
                id);


        if (note is null)
        {
            throw new ApiException(
                404,
                "Запись не найдена или удалена");
        }


        note.Title =
            NormalizeTitle(
                request.Title);

        note.Text =
            NormalizeText(
                request.Text);

        note.Category =
            NormalizeOptionalText(
                request.Category);

        note.IsPinned =
            request.IsPinned;

        note.UpdatedAt =
            DateTimeOffset.UtcNow;


        return _noteRepository.Update(
            note);
    }


    public void Delete(
        int userId,
        int id)
    {
        if (!_noteRepository.Delete(
                userId,
                id))
        {
            throw new ApiException(
                404,
                "Запись уже удалена");
        }
    }


    private static string? NormalizeTitle(
        string? title)
    {
        if (string.IsNullOrWhiteSpace(
                title))
        {
            return null;
        }


        var normalized =
            title.Trim();


        if (normalized.Length >
            TitleMaxLength)
        {
            throw new ApiException(
                400,
                $"Заголовок не должен быть длиннее {TitleMaxLength} символов");
        }


        return normalized;
    }


    private static string NormalizeText(
        string text)
    {
        var normalized =
            text.Trim();


        if (string.IsNullOrWhiteSpace(
                normalized))
        {
            throw new ApiException(
                400,
                "Напишите хотя бы пару слов");
        }


        if (normalized.Length >
            TextMaxLength)
        {
            throw new ApiException(
                400,
                $"Текст заметки не должен быть длиннее {TextMaxLength} символов");
        }


        return normalized;
    }


    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }
}