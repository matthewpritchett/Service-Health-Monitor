using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Service_Health.Core.Models;
using Service_Health.InMemory;

namespace Service_Health.Tests;

[TestFixture]
public class InMemoryCheckResultStoreTests
{
    private readonly ILogger<InMemoryCheckResultStore> _logger =
        NullLogger<InMemoryCheckResultStore>.Instance;

    [Test]
    public void Save_ShouldStoreResult()
    {
        // Arrange
        var store = new InMemoryCheckResultStore(_logger);

        var result = new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = true,
            Timestamp = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        // Act
        store.Save(result);

        // Assert
        var history = store.GetHistory(result.Key);

        Assert.That(history, Has.Count.EqualTo(1));
        Assert.That(history[0], Is.SameAs(result));
    }

    [Test]
    public void GetLatest_ShouldReturnLatestResultPerService()
    {
        // Arrange
        var store = new InMemoryCheckResultStore(_logger);

        var older = new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = false,
            Timestamp = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        var newer = new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = true,
            Timestamp = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)
        };

        var other = new CheckResult
        {
            GroupName = "service",
            ServiceName = "b",
            Success = true,
            Timestamp = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc)
        };

        store.Save(older);
        store.Save(newer);
        store.Save(other);

        // Act
        var latest = store.GetLatest();

        // Assert
        Assert.That(latest, Has.Count.EqualTo(2));
        Assert.That(latest.Any(x => x.Key == newer.Key && ReferenceEquals(x, newer)), Is.True);
        Assert.That(latest.Any(x => x.Key == other.Key && ReferenceEquals(x, other)), Is.True);
    }

    [Test]
    public void GetHistory_ShouldReturnResultsInDescendingTimestampOrder()
    {
        // Arrange
        var store = new InMemoryCheckResultStore(_logger);

        var oldest = new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = true,
            Timestamp = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc)
        };

        var middle = new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = false,
            Timestamp = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        var newest = new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = true,
            Timestamp = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)
        };

        store.Save(middle);
        store.Save(oldest);
        store.Save(newest);

        // Act
        var history = store.GetHistory(newest.Key);

        // Assert
        Assert.That(history, Has.Count.EqualTo(3));
        Assert.That(history[0], Is.SameAs(newest));
        Assert.That(history[1], Is.SameAs(middle));
        Assert.That(history[2], Is.SameAs(oldest));
    }

    [Test]
    public void GetHistory_ShouldReturnOnlyRequestedService()
    {
        // Arrange
        var store = new InMemoryCheckResultStore(_logger);

        store.Save(new CheckResult
        {
            GroupName = "service",
            ServiceName = "a",
            Success = true,
            Timestamp = DateTime.UtcNow
        });

        store.Save(new CheckResult
        {
            GroupName = "service",
            ServiceName = "b",
            Success = true,
            Timestamp = DateTime.UtcNow
        });

        // Act
        var history = store.GetHistory("service-a");

        // Assert
        Assert.That(history, Has.Count.EqualTo(1));
        Assert.That(history[0].Key, Is.EqualTo("service-a"));
    }

    [Test]
    public void GetHistory_ForUnknownService_ShouldReturnEmptyCollection()
    {
        // Arrange
        var store = new InMemoryCheckResultStore(_logger);

        // Act
        var history = store.GetHistory("does-not-exist");

        // Assert
        Assert.That(history, Is.Empty);
    }

    [Test]
    public void GetLatest_WhenEmpty_ShouldReturnEmptyCollection()
    {
        // Arrange
        var store = new InMemoryCheckResultStore(_logger);

        // Act
        var latest = store.GetLatest();

        // Assert
        Assert.That(latest, Is.Empty);
    }
}