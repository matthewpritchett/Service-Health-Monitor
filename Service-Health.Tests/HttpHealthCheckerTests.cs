using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Service_Health.Checkers;
using Service_Health.Core.Models;

namespace Service_Health.Tests;

[TestFixture]
public class HttpHealthCheckerTests
{
    [Test]
    public async Task CheckAsync_WhenStatusMatchesExpected_ReturnsSuccess()
    {
        var httpClient = new HttpClient(
            new TestHttpMessageHandler(HttpStatusCode.OK));

        var checker = new HttpHealthChecker(
            httpClient,
            NullLogger<HttpHealthChecker>.Instance);

        var service = new ServiceDefinition
        {
            Type = "http",
            Name = "Google",
            Url = new Uri("https://google.com"),
            ExpectedStatus = 200
        };

        var result = await checker.CheckAsync("Web", service);

        Assert.That(result.Success, Is.True);
        Assert.That(result.GroupName, Is.EqualTo("Web"));
        Assert.That(result.ServiceName, Is.EqualTo("Google"));
        Assert.That(result.Message, Is.Null);
    }

    [Test]
    public async Task CheckAsync_WhenStatusDoesNotMatchExpected_ReturnsFailure()
    {
        var httpClient = new HttpClient(
            new TestHttpMessageHandler(HttpStatusCode.NotFound));

        var checker = new HttpHealthChecker(
            httpClient,
            NullLogger<HttpHealthChecker>.Instance);

        var service = new ServiceDefinition
        {
            Type = "http",
            Name = "Missing",
            Url = new Uri("https://example.com"),
            ExpectedStatus = 200
        };

        var result = await checker.CheckAsync("Web", service);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Status code: 404"));
    }

    private sealed class TestHttpMessageHandler(HttpStatusCode statusCode)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(statusCode));
        }
    }
}