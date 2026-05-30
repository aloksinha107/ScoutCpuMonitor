using Microsoft.Extensions.Configuration;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;
using ScoutMonitor.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ScoutMonitor.Application;

public class MonitoringService
{
    private readonly ISystemMonitor _systemMonitor;
    private readonly IEnumerable<IMonitorPlugin> _plugins;
    private readonly MonitoringSettings _settings;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(
        ISystemMonitor systemMonitor,
        IEnumerable<IMonitorPlugin> plugins,
        IOptions<MonitoringSettings> settings,
        ILogger<MonitoringService> logger)
    {
        _systemMonitor = systemMonitor;
        _plugins = plugins;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
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
                        _logger.LogError(ex,
                            "Plugin {PluginName} failed",
                            plugin.GetType().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Monitoring cycle failed");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(_settings.IntervalSeconds),
                cancellationToken);
        }
    }

    private void PrintMetrics(SystemMetrics metrics)
    {
        Console.WriteLine();
        Console.WriteLine("====================================");
        Console.WriteLine($"Timestamp : {metrics.Timestamp:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"CPU       : {metrics.CpuUsagePercent:F2}%");
        Console.WriteLine($"RAM       : {metrics.RamUsedMb:F2} MB / {metrics.RamTotalMb:F2} MB");
        Console.WriteLine($"DISK      : {metrics.DiskUsedGb:F2} GB / {metrics.DiskTotalGb:F2} GB");
        Console.WriteLine("========================================");
    }
}