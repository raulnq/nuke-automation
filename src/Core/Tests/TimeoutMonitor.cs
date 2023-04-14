using System.Runtime.ExceptionServices;
using Shouldly;

namespace Core.Tests;

public class TimeoutMonitor
{
    private readonly HashSet<Type> _retryableExceptions;

    public TimeoutMonitor() : this(new[] { typeof(ShouldAssertException) })
    {

    }

    public TimeoutMonitor(IEnumerable<Type> retryableExceptions)
    {
        _retryableExceptions = retryableExceptions.ToHashSet();
    }

    private string TimeoutError(TimeSpan timeout) => $"Task timeout after {timeout}";

    public async Task RunUntil(Func<Task> taskFactory, TimeSpan timeout, TimeSpan interval)
    {
        var cts = new CancellationTokenSource(timeout);

        Exception? lastException = null;

        var task = taskFactory();

        while (true)
        {
            try
            {
                cts.Token.ThrowIfCancellationRequested();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return;
                }

                if (task.Status == TaskStatus.Faulted)
                {
                    task.GetAwaiter().GetResult();
                }

                await Task.Delay(interval, cts.Token);

            }
            catch (OperationCanceledException)
            {
                if (lastException != null)
                {
                    ExceptionDispatchInfo.Capture(lastException).Throw();

                }

                throw new TimeoutException(TimeoutError(timeout));

            }
            catch (Exception ex) when (_retryableExceptions.Contains(ex.GetType()))
            {
                lastException = ex;

                task = taskFactory();
            }
        }
    }
}