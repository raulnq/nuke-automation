namespace Core.Infrastructure;

public static class CustomSchemaIdProvider
{
    public static string Get(Type type)
    {
        if (type.IsGenericType)
        {
            return PrettyTypeName(type);
        }

        return GetName(type);
    }

    private static string GetName(Type type)
    {
        return type.FullName!.Replace("+", ".");
    }

    private static string PrettyTypeName(Type type)
    {
        if (type.IsArray)
        {
            return PrettyTypeName(type.GetElementType()!) + "[]";
        }

        if (type.IsGenericType)
        {
            return string.Format(
                "{0}<{1}>",
                GetName(type).Substring(0, GetName(type).LastIndexOf("`", StringComparison.InvariantCulture)),
                string.Join(", ", type.GetGenericArguments().Select(PrettyTypeName)));
        }

        return GetName(type);
    }
}
