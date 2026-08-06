using Service_Health.Checkers;
using Service_Health.Core;
using Service_Health.Host;
using Service_Health.InMemoryCheckResultStore;
using Service_Health.YamlMonitorConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<IMonitorConfigurationSource>(
    sp => new YamlMonitorConfigurationSource(
        "servicemonitor.yaml",
        sp.GetRequiredService<ILogger<YamlMonitorConfigurationSource>>()));

builder.Services.AddSingleton<ICheckResultStore, InMemoryCheckResultStore>();

builder.Services.AddHttpClient<IHealthChecker, HttpHealthChecker>();
builder.Services.AddSingleton<IHealthChecker, PingHealthChecker>();

builder.Services.AddHostedService<MonitorWorker>();

var app = builder.Build();

app.UseStaticFiles();

app.MapRazorPages();

app.Run();