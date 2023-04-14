using Core.Domain;

namespace Core.Application;

public class ApplicationException : BaseException
{
    public ApplicationException(string code) : base(code)
    {
    }

    public ApplicationException(string code, params object[] parameters) : base(code, parameters)
    {
    }

    public ApplicationException(string code, string description) : base(code, description)
    {
    }

    public ApplicationException(Type type) : base(type.Name.Replace("Exception", ""))
    {
    }

    public ApplicationException(Type type, object[] parameters) : base(type.Name.Replace("Exception", ""), parameters)
    {
    }
}
