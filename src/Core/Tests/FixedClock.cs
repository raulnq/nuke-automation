using Core.Domain;

namespace Core.Tests;

public class FixedClock : IClock
{
    public FixedClock(DateTimeOffset now)
    {
        Now = new DateTimeOffset(now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            now.Millisecond,
            now.Offset);
    }

    public FixedClock() : this(DateTimeOffset.UtcNow)
    {
    }

    public DateTimeOffset Now { get; private set; }

    public void TravelTo(DateTimeOffset now)
    {
        Now = new DateTimeOffset(now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            now.Millisecond,
            now.Offset);
    }
}
