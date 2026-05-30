using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Domain.Interfaces;

public interface IMonitorPlugin
{
    Task ProcessAsync(SystemMetrics metrics);
}