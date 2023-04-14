namespace Core.Domain;

public interface IClock
{
    DateTimeOffset Now { get; }
}
