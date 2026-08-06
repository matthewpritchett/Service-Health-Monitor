namespace Service_Health.Core.Models;

public class ServiceDefinition
{
    public required string Type { get; init; }

    public required string Name { get; init; }

    public Uri? Url { get; init; }

    public string? Host { get; init; }

    public int ExpectedStatus { get; init; } = 200;
}