namespace Service_Health.Core.Models;

public class HttpServiceDefinition : ServiceDefinition
{
    public required Uri Url { get; init; }
    public int ExpectedStatus { get; init; } = 200;
}