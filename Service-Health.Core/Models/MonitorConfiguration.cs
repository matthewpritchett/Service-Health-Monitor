namespace Service_Health.Core.Models;

public class MonitorConfiguration
{
    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(30);

    public List<ServiceGroup> ServiceGroups { get; init; } = [];
}