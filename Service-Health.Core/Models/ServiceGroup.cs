namespace Service_Health.Core.Models;

public class ServiceGroup
{
    public string Name { get; init; } = "";
    public List<ServiceDefinition> ServiceDefinitions { get; init; } = [];
}