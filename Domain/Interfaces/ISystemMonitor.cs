using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Domain.Interfaces;

public interface ISystemMonitor
{
    Task<SystemMetrics> GetMetricsAsync();
}