namespace Core.Domain;

public interface IRepository<T> where T : class
{
    Task<T> Get(params object[] keyValues);

    void Add(T entity);

    void Add(T[] entity);

    void Remove(T entity);

    Task<int> GetNextValue();

    Task<long> GetNextLongValue();
}
