namespace Core.Domain;

public class DomainException : BaseException
{
    public DomainException(string code, params object[] parameters) : base(code, parameters)
    {
    }

    public DomainException(string code, string description) : base(code, description)
    {
    }

    public DomainException(Type type) : base(type.Name.Replace("Exception", ""))
    {
    }

    public DomainException(Type type, object[] parameters) : base(type.Name.Replace("Exception", ""), parameters)
    {
    }
}