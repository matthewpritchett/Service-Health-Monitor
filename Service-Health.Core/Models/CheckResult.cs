namespace Service_Health.Core.Models;

public class CheckResult
{
    public string Key => $"{GroupName}-{ServiceName}";
    
    public string GroupName { get; init; } = "";

    public string ServiceName { get; init; } = "";

    public DateTimeOffset Timestamp { get; init; }

    public bool Success { get; init; }
    
    public string? Message { get; init; }
}