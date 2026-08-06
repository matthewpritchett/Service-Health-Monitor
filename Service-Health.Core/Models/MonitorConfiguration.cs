namespace Service_Health.Core.Models;

public class MonitorConfiguration
{
    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(30);

    public List<ServiceGroup> Groups { get; init; } = [];
}