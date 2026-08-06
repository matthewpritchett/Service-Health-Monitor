using Microsoft.Extensions.Logging;
using Service_Health.Core;
using Service_Health.Core.Models;

namespace Service_Health.Checkers;

public class HttpHealthChecker(
    HttpClient httpClient,
    ILogger<HttpHealthChecker> logger) : IHealthChecker
{
    public string Type => "http";
    public async Task<CheckResult> CheckAsync(string groupName, ServiceDefinition service)
    {
        logger.LogInformation(
            "Checking HTTP service {Group}/{Service} at {Url}",
            groupName,
            service.Name,
            service.Url);

        try
        {
            using var response = await httpClient.GetAsync(service.Url);

            var success = (int)response.StatusCode == service.ExpectedStatus;

            if (success)
            {
                logger.LogInformation(
                    "HTTP check succeeded for {Group}/{Service}. Status: {StatusCode}",
                    groupName,
                    service.Name,
                    (int)response.StatusCode);
            }
            else
            {
                logger.LogWarning(
                    "HTTP check failed for {Group}/{Service}. Status: {StatusCode}, Expected: {ExpectedStatus}",
                    groupName,
                    service.Name,
                    (int)response.StatusCode,
                    service.ExpectedStatus);
            }

            return new CheckResult
            {
                GroupName = groupName,
                ServiceName = service.Name,
                Success = success,
                Timestamp = DateTimeOffset.UtcNow,
                Message = success
                    ? null
                    : $"Status code: {(int)response.StatusCode}, expected: {service.ExpectedStatus}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "HTTP check failed with exception for {Group}/{Service}",
                groupName,
                service.Name);

            return new CheckResult
            {
                GroupName = groupName,
                ServiceName = service.Name,
                Success = false,
                Timestamp = DateTimeOffset.UtcNow,
                Message = ex.Message
            };
        }
    }
}