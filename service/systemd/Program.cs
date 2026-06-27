using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var service = Host
    .CreateDefaultBuilder(args)
    .UseSystemd()
    .ConfigureServices((hostContext, services) =>
    {
        _ = services.AddHostedService<Service>();
    })
    .Build();

await service.RunAsync().ConfigureAwait(false);

#pragma warning disable CA1812
internal sealed class Service : BackgroundService
#pragma warning restore CA1812
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);
        }
    }
}
