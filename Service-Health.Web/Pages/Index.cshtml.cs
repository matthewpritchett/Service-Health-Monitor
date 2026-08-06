using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service_Health.Core;
using Service_Health.Core.Models;

namespace Service_Health.Web.Pages;

public class Index(
    ICheckResultStore checkResultStore,
    ILogger<Index> logger,
    IMonitorConfigurationSource monitorConfigurationSource)
    : PageModel
{
    public TimeSpan RefreshInterval { get; private set; }
    public IReadOnlyList<CheckResult> Results { get; private set; } = [];

    public void OnGet()
    {
        var configuration = monitorConfigurationSource.GetConfiguration();

        RefreshInterval = configuration.Interval;
        
        Results = checkResultStore.GetLatest();
        
        logger.LogInformation(
            "Status page loaded with {ServiceCount} services",
            Results.Count);

        var failures = Results.Count(x => !x.Success);

        if (failures > 0)
        {
            logger.LogWarning(
                "Status page contains {FailureCount} failed services",
                failures);
        }
    }
}