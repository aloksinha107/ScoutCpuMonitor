using Microsoft.Extensions.Configuration;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Application;

public class MonitoringService
{
    private readonly ISystemMonitor _systemMonitor;
    private readonly IEnumerable<IMonitorPlugin> _plugins;
    private readonly IConfiguration _configuration;

    public MonitoringService(
        ISystemMonitor systemMonitor,
        IEnumerable<IMonitorPlugin> plugins,
        IConfiguration configuration)
    {
        _systemMonitor = systemMonitor;
        _plugins = plugins;
        _configuration = configuration;
    }

    public async Task RunAsync()
    {
        int intervalSeconds = _configuration.GetValue<int>("Monitoring:IntervalSeconds");

        while (true)
        {
            try
            {
                var metrics = await _systemMonitor.GetMetricsAsync();

                PrintMetrics(metrics);

                foreach (var plugin in _plugins)
                {
                    try
                    {
                        await plugin.ProcessAsync(metrics);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Plugin failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Monitoring error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));
        }
    }

    private void PrintMetrics(SystemMetrics metrics)
    {
        Console.WriteLine("====================================");
        Console.WriteLine($"Timestamp : {metrics.Timestamp}");
        Console.WriteLine($"CPU Usage : {metrics.CpuUsagePercent:F2}%");
        Console.WriteLine($"RAM Usage : {metrics.RamUsedMb:F2} MB / {metrics.RamTotalMb:F2} MB");
        Console.WriteLine($"Disk Usage: {metrics.DiskUsedGb:F2} GB / {metrics.DiskTotalGb:F2} GB");
    }
}