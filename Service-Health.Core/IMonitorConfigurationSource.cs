using Service_Health.Core.Models;

namespace Service_Health.Core;

public interface IMonitorConfigurationSource
{
    MonitorConfiguration GetConfiguration();
}