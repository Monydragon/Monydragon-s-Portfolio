namespace Monydragons_Portfolio.Services.Utility.Interface;

public interface IPollingService
{
    void StartPolling(TimeSpan interval, Func<Task>? callbackAsync);
    void StopPolling();
}