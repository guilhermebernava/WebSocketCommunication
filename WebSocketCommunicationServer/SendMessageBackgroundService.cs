using Microsoft.Extensions.Hosting;

public class SendMessageBackgroundService : BackgroundService
{
    private readonly CancellationTokenSource _cancelToken = new();
    private int times = 0;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => _cancelToken.Cancel());
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_cancelToken.Token.IsCancellationRequested || times > 1) break;

            await Console.Out.WriteLineAsync("Running - " + DateTime.UtcNow);
            times++;
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    public void Stop() => _cancelToken.Cancel();

}
