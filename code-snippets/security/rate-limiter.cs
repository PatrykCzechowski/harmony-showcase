// Context: Limiting repeated sensitive actions (login, MFA, AI calls) in sliding window.
// Problem: Brute force / abuse can degrade service or compromise accounts.
// Solution: Lightweight in-memory window + lockout timestamp (extensible to distributed cache).
// Value: Predictable throttling, minimal overhead, secure defaults.

public sealed class SlidingWindowRateLimiter
{
    private readonly TimeSpan _window;
    private readonly int _maxAttempts;
    private readonly TimeSpan _lockout;
    private readonly ConcurrentDictionary<string, Entry> _entries = new();
    private readonly ISystemClock _clock;

    public SlidingWindowRateLimiter(int maxAttempts, TimeSpan window, TimeSpan lockout, ISystemClock clock)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(window.TotalMilliseconds);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lockout.TotalMilliseconds);
        ArgumentNullException.ThrowIfNull(clock);
        
        _maxAttempts = maxAttempts;
        _window = window;
        _lockout = lockout;
        _clock = clock;
    }

    public Evaluation Register(string key, bool isSuccessful)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var now = _clock.UtcNow;
        var entry = _entries.GetOrAdd(key, _ => new Entry());
        
        lock (entry)
        {
            if (IsCurrentlyLocked(entry, now))
                return CreateLockedEvaluation(entry, now);

            PruneExpiredAttempts(entry, now);
            UpdateAttemptHistory(entry, now, isSuccessful);

            return ShouldLock(entry) 
                ? LockAndCreateEvaluation(entry, now)
                : CreateAllowedEvaluation(entry);
        }
    }
    
    private bool IsCurrentlyLocked(Entry entry, DateTime now) =>
        entry.LockedUntil.HasValue && entry.LockedUntil > now;
    
    private Evaluation CreateLockedEvaluation(Entry entry, DateTime now) =>
        new(true, (entry.LockedUntil - now) ?? TimeSpan.Zero, entry.Attempts.Count);
    
    private void PruneExpiredAttempts(Entry entry, DateTime now) =>
        entry.Attempts.RemoveAll(attempt => now - attempt > _window);
    
    private void UpdateAttemptHistory(Entry entry, DateTime now, bool isSuccessful)
    {
        if (isSuccessful)
            entry.Attempts.Clear();
        else
            entry.Attempts.Add(now);
    }
    
    private bool ShouldLock(Entry entry) => entry.Attempts.Count >= _maxAttempts;
    
    private Evaluation LockAndCreateEvaluation(Entry entry, DateTime now)
    {
        entry.LockedUntil = now + _lockout;
        return new Evaluation(true, _lockout, entry.Attempts.Count);
    }
    
    private static Evaluation CreateAllowedEvaluation(Entry entry) =>
        new(false, TimeSpan.Zero, entry.Attempts.Count);

    private sealed class Entry
    {
        public List<DateTime> Attempts { get; } = new();
        public DateTime? LockedUntil { get; set; }
    }

    public record Evaluation(bool IsLocked, TimeSpan RetryAfter, int AttemptsInWindow);
}

public interface ISystemClock
{
    DateTime UtcNow { get; }
}

public sealed class SystemClock : ISystemClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
