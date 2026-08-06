using Service_Health.Core;

namespace Service_Health.Host;

public class MonitorWorker(
    IMonitorConfigurationSource configurationSource,
    IEnumerable<IHealthChecker> checkers,
    ICheckResultStore store)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var configuration = configurationSource.GetConfiguration();

            foreach (var group in configuration.Groups)
            {
                foreach (var service in group.Services)
                {
                    var checker = checkers.First(x => x.Type == service.Type);

                    var result = await checker.CheckAsync(group.Name, service);

                    store.Save(result);
                }
            }

            await Task.Delay(configuration.Interval, stoppingToken);
        }
    }
}