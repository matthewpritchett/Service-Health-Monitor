using Microsoft.Extensions.Logging;
using Service_Health.Core;
using Service_Health.Core.Models;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Service_Health.YamlMonitorConfiguration;

public class YamlMonitorConfigurationSource(string path, ILogger<YamlMonitorConfigurationSource> logger)
    : IMonitorConfigurationSource
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance) .Build();

    public MonitorConfiguration GetConfiguration()
    {
        logger.LogInformation("Loading monitor configuration from {Path}", path);
        
        try
        {
            var yaml = File.ReadAllText(path);

            var configuration = _deserializer.Deserialize<MonitorConfiguration>(yaml) ?? throw new InvalidOperationException("Configuration file was empty.");

            logger.LogInformation(
                "Loaded monitor configuration. Groups: {GroupCount}, Services: {ServiceCount}, Interval: {Interval}",
                configuration.Groups.Count,
                configuration.Groups.Sum(x => x.Services.Count),
                configuration.Interval);
            
            return configuration;
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "Configuration file not found: {Path}", path);
            throw;
        }
        catch (YamlException ex)
        {
            logger.LogError(ex, "Invalid YAML configuration: {Path}", path);
            throw;
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "Unable to read configuration file: {Path}", path);
            throw;
        }
    }
}