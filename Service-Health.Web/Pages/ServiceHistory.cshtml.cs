using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service_Health.Core;
using Service_Health.Core.Models;

namespace Service_Health.Web.Pages;

public class ServiceHistory(
    ICheckResultStore checkResultStore,
    ILogger<ServiceHistory> logger,
    IMonitorConfigurationSource monitorConfigurationSource) : PageModel
{
    public TimeSpan RefreshInterval { get; private set; }
    public IReadOnlyList<CheckResult> Results { get; private set; } = [];

    public string Key { get; private set; } = "";
    public void OnGet(string key)
    {
        logger.LogInformation(
            "Loading service history for {ServiceKey}",
            key);
        var configuration = monitorConfigurationSource.GetConfiguration();
        RefreshInterval = configuration.Interval;
        
        logger.LogInformation(
            "Loaded service history for {ServiceKey}. Results: {ResultCount}",
            key,
            Results.Count);
        Key = key;
        Results = checkResultStore.GetHistory(key);
        
        if (Results.Count == 0)
        {
            logger.LogWarning(
                "No history found for service {ServiceKey}",
                key);
            return;
        }

        var failures = Results.Count(x => !x.Success);

        if (failures > 0)
        {
            logger.LogWarning(
                "Service history for {ServiceKey} contains {FailureCount} failed checks",
                key,
                failures);
        }
    }
}