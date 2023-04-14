using Rebus.Bus;
using Serilog;
using Core.Application;

namespace Core.Infrastructure;

public class RebusCommandSender : ICommandSender
{
    private readonly IBus _bus;

    private static readonly Serilog.ILogger _logger = Log.Logger.ForContext<RebusCommandSender>();

    public RebusCommandSender(IBus bus)
    {
        _bus = bus;
    }

    public async Task Send(ICommand command, TimeSpan deferTimeSpan = default)
    {
        try
        {
            var commandTypeName = command.GetType().Name;

            _logger.Debug("Sending command {commandTypeName}", commandTypeName);

            if (deferTimeSpan == default)
            {
                await _bus.Send(command);
            }
            else
            {
                await _bus.Defer(deferTimeSpan, command);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error sending command");
        }
    }
}
