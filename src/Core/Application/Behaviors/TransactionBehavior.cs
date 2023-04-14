using MediatR;

namespace Core.Application;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();

        if (request is IQuery)
        {
            var result = await next();

            return result;
        }

        if (_unitOfWork.IsTransactionOpened())
        {
            var result = await next();

            return result;
        }

        TResponse response;

        try
        {
            await _unitOfWork.BeginTransaction();

            response = await next();

            await _unitOfWork.Commit();

            return response;
        }
        catch
        {
            await _unitOfWork.Rollback();

            throw;
        }
    }


}
