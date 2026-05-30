using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Infrastructure.Monitoring;

public class WindowsSystemMonitor : ISystemMonitor
{
    private readonly PerformanceCounter _cpuCounter;

    public WindowsSystemMonitor()
    {
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _cpuCounter.NextValue();
    }

    public async Task<SystemMetrics> GetMetricsAsync()
    {
        await Task.Delay(500);

        double cpuUsage = _cpuCounter.NextValue();

        var computerInfo = new ComputerInfo();

        double totalRamMb = computerInfo.TotalPhysicalMemory / 1024.0 / 1024.0;
        double availableRamMb = computerInfo.AvailablePhysicalMemory / 1024.0 / 1024.0;
        double usedRamMb = totalRamMb - availableRamMb;

        var drive = DriveInfo.GetDrives()
            .First(d => d.IsReady && d.DriveType == DriveType.Fixed);

        double totalDiskGb = drive.TotalSize / 1024.0 / 1024.0 / 1024.0;
        double freeDiskGb = drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
        double usedDiskGb = totalDiskGb - freeDiskGb;

        return new SystemMetrics
        {
            CpuUsagePercent = cpuUsage,
            RamUsedMb = usedRamMb,
            RamTotalMb = totalRamMb,
            DiskUsedGb = usedDiskGb,
            DiskTotalGb = totalDiskGb,
            Timestamp = DateTime.UtcNow
        };
    }
}