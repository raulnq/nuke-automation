namespace Core.Domain;

public class NotFoundException : BaseException
{
    public NotFoundException() : base("resource-not-found")
    {
    }

    public NotFoundException(string code) : base(code)
    {
    }

    protected NotFoundException(string code, string description) : base(code, description)
    {
    }

    protected NotFoundException(string code, object[] parameters) : base(code, parameters)
    {
    }
}

public class NotFoundException<TEntity> : NotFoundException
{
    public NotFoundException() : base($"{typeof(TEntity).Name.ToLower()}-not-found")
    {
    }

    public NotFoundException(params object[] keyValues) : base($"{typeof(TEntity).Name.ToLower()}-not-found", keyValues)
    {
    }

    protected NotFoundException(string code, object[] parameters) : base(code, parameters)
    {
    }

    protected NotFoundException(string code, string description) : base(code, description)
    {
    }

    protected NotFoundException(string code) : base(code)
    {
    }
}
