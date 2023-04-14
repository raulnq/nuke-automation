using Core.Domain;

namespace Core.Infrastructure;

public class InfrastructureException : BaseException
{
    public InfrastructureException(string code, object[] parameters) : base(code, parameters)
    {
    }

    public InfrastructureException(string code, string description) : base(code, description)
    {
    }

    public InfrastructureException(string code) : base(code)
    {
    }

    public InfrastructureException(Type type) : base(type.Name.Replace("Exception", ""))
    {
    }

    public InfrastructureException(Type type, object[] parameters) : base(type.Name.Replace("Exception", ""), parameters)
    {
    }
}