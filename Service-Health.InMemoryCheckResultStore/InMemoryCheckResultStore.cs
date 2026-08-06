using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Service_Health.Core;
using Service_Health.Core.Models;

namespace Service_Health.InMemoryCheckResultStore;

public class InMemoryCheckResultStore(ILogger<InMemoryCheckResultStore> logger) : ICheckResultStore
{
    private readonly ILogger<InMemoryCheckResultStore> _logger = logger;
    private readonly ConcurrentBag<CheckResult> _results = [];

    public void Save(CheckResult result)
    {
        _results.Add(result);
        
        _logger.LogInformation(
            "Saved check result. Service: {Key}, Success: {Success}, Timestamp: {Timestamp}",
            result.Key,
            result.Success,
            result.Timestamp);
    }

    public IReadOnlyList<CheckResult> GetLatest()
    {
        var latest = _results
            .GroupBy(x => x.Key)
            .Select(x => x.MaxBy(r => r.Timestamp)!)
            .ToList();

        _logger.LogInformation(
            "Retrieved latest check results. Services: {Count}",
            latest.Count);

        return latest;
    }

    public IReadOnlyList<CheckResult> GetHistory(string key)
    {
        var history = _results
            .Where(x => x.Key == key)
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        _logger.LogInformation(
            "Retrieved check history. Service: {Key}, Results: {Count}",
            key,
            history.Count);

        return history;
    }
}