using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using Service_Health.Core;
using Service_Health.Core.Models;

namespace Service_Health.Checkers;

public class PingHealthChecker(
    ILogger<PingHealthChecker> logger) : IHealthChecker
{
    public string Type => "ping";

    public async Task<CheckResult> CheckAsync(string groupName, ServiceDefinition service)
    {
        logger.LogInformation(
            "Checking ping service {Group}/{Service} at {Host}",
            groupName,
            service.Name,
            service.Host);

        try
        {
            using var ping = new Ping();

            var reply = await ping.SendPingAsync(service.Host);

            var success = reply.Status == IPStatus.Success;

            if (success)
            {
                logger.LogInformation(
                    "Ping check succeeded for {Group}/{Service}. Round trip time: {RoundTripTime}ms",
                    groupName,
                    service.Name,
                    reply.RoundtripTime);
            }
            else
            {
                logger.LogWarning(
                    "Ping check failed for {Group}/{Service}. Status: {Status}",
                    groupName,
                    service.Name,
                    reply.Status);
            }

            return new CheckResult
            {
                GroupName = groupName,
                ServiceName = service.Name,
                Success = success,
                Timestamp = DateTimeOffset.UtcNow,
                Message = success
                    ? $"Ping: {reply.RoundtripTime}ms"
                    : $"Ping failed: {reply.Status}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ping check failed with exception for {Group}/{Service}",
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