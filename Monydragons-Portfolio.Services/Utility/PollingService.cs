using Monydragons_Portfolio.Services.Utility.Interface;

namespace Monydragons_Portfolio.Services.Utility;

public class PollingService : IPollingService
{
    protected Timer _timer;
    
    public virtual void StartPolling(TimeSpan interval, Func<Task>? callbackAsync)
    {
        _timer?.Dispose(); // Dispose any existing timer

        // No direct async support in Timer, so use workaround
        _timer = new Timer(async _ =>
            {
                await callbackAsync?.Invoke()!;
            }, 
            null, 
            interval, 
            Timeout.InfiniteTimeSpan); // Prevent reentrancy
    }

    public virtual void StopPolling()
    {
        _timer?.Dispose();
    }
}