using StudyHub_Project.Server.Models;
using System.Collections.Concurrent;

namespace StudyHub_Project.Server.Data;

public class InMemoryDataStore
{
    private int _nextUserId;
    private int _nextTaskId;
    private int _nextNoteId;
    private int _nextFocusSessionId;


    public ConcurrentDictionary<int, UserAccount> Users { get; }
        = new();

    public ConcurrentDictionary<int, StudyTask> Tasks { get; }
        = new();

    public ConcurrentDictionary<int, Note> Notes { get; }
        = new();

    public ConcurrentDictionary<int, FocusSession> FocusSessions { get; }
        = new();

    public ConcurrentDictionary<int, Campfire> Campfires { get; }
        = new();

    public ConcurrentDictionary<string, int> Sessions { get; }
        = new(StringComparer.Ordinal);


    public ConcurrentDictionary<string, LoginAttemptState> LoginAttempts
    { get; }
        = new(StringComparer.OrdinalIgnoreCase);


    public object AuthSyncRoot { get; } = new();

    public object TaskCampfireSyncRoot { get; } = new();

    public object FocusSyncRoot { get; } = new();


    public int GetNextUserId()
    {
        return Interlocked.Increment(
            ref _nextUserId);
    }

    public int GetNextTaskId()
    {
        return Interlocked.Increment(
            ref _nextTaskId);
    }

    public int GetNextNoteId()
    {
        return Interlocked.Increment(
            ref _nextNoteId);
    }

    public int GetNextFocusSessionId()
    {
        return Interlocked.Increment(
            ref _nextFocusSessionId);
    }
}