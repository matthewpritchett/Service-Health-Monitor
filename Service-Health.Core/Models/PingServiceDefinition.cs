namespace Service_Health.Core.Models;

public class PingServiceDefinition : ServiceDefinition
{
    public required string Host { get; init; }
}