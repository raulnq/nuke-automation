namespace Core.Application;

public interface IUnitOfWork
{
    bool IsTransactionOpened();

    Task BeginTransaction();

    Task Rollback();

    Task Commit();
}
