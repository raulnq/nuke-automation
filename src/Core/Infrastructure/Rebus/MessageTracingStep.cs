using Rebus.Pipeline;
using Serilog.Context;

namespace Core.Infrastructure;

[StepDocumentation("Adding TransactionId")]
public class MessageTracingStep : IIncomingStep
{
    public async Task Process(IncomingStepContext context, Func<Task> next)
    {
        var transactionId = Guid.NewGuid().ToString();

        using (LogContext.PushProperty("TransactionId", transactionId))
        {
            await next();
        }
    }
}
