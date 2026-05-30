namespace ScoutMonitor.Domain.Models;

public class SystemMetrics
{
    public double CpuUsagePercent { get; set; }

    public double RamUsedMb { get; set; }
    public double RamTotalMb { get; set; }

    public double DiskUsedGb { get; set; }
    public double DiskTotalGb { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}