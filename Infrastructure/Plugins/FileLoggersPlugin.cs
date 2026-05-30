using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Infrastructure.Plugins;

public class FileLoggerPlugin : IMonitorPlugin
{
    private readonly string _filePath = "metrics.log";

    public async Task ProcessAsync(SystemMetrics metrics)
    {
        string log =
            $"[{metrics.Timestamp}] CPU={metrics.CpuUsagePercent:F2}% " +
            $"RAM={metrics.RamUsedMb:F2}/{metrics.RamTotalMb:F2} MB " +
            $"DISK={metrics.DiskUsedGb:F2}/{metrics.DiskTotalGb:F2} GB";

        await File.AppendAllTextAsync(_filePath, log + Environment.NewLine);
    }
}