using Service_Health.Core.Models;

namespace Service_Health.Core;

public interface IHealthChecker
{
    string Type { get; }
    Task<CheckResult> CheckAsync(string groupName, ServiceDefinition service);
}