namespace Core.Domain;

public class DuplicatedException<TEntity> : DomainException
{
    public DuplicatedException() : base($"{typeof(TEntity).Name.ToLower()}-duplicated")
    {
    }

    public DuplicatedException(params object[] keyValues) : base($"{typeof(TEntity).Name.ToLower()}-duplicated", keyValues)
    {
    }

    protected DuplicatedException(string code, object[] parameters) : base(code, parameters)
    {
    }

    protected DuplicatedException(string code, string description) : base(code, description)
    {
    }

    protected DuplicatedException(string code) : base(code)
    {
    }
}