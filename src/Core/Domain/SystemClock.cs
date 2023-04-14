namespace Core.Domain;

public class SystemClock : IClock
{
    public DateTimeOffset Now => GetNow();

    private DateTimeOffset GetNow()
    {
        var now = DateTimeOffset.UtcNow;

        return new DateTimeOffset(now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            now.Millisecond,
            now.Offset);
    }
}
