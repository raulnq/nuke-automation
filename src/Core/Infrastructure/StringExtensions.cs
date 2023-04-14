namespace WebAPI.Infrastructure;

public static class StringExtensions
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}
