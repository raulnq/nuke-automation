namespace Core.Tests;

public abstract class Dsl
{
    protected readonly TimeoutMonitor _timeoutMonitor;
    protected readonly static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
    protected readonly static TimeSpan _defaultInterval = TimeSpan.FromSeconds(30);
    protected Dsl()
    {
        _timeoutMonitor = new TimeoutMonitor();
    }

    public Task WaitFor(Func<Task> taskFactory, TimeSpan timeout, TimeSpan interval) => _timeoutMonitor.RunUntil(taskFactory, timeout, interval);
}
