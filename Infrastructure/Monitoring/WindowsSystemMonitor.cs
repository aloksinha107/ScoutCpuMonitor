using System.Diagnostics;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;
using System.Management;

namespace ScoutMonitor.Infrastructure.Monitoring;

/// <summary>
/// Windows-specific implementation of system monitoring.
/// Future Linux/macOS implementations can be added by
/// implementing ISystemMonitor.
/// </summary>
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

        //var computerInfo = new ComputerInfo();

        var (usedRamMb, totalRamMb) = GetMemoryUsage();
        //double totalRamMb = computerInfo.TotalPhysicalMemory / 1024.0 / 1024.0;
        //double availableRamMb = computerInfo.AvailablePhysicalMemory / 1024.0 / 1024.0;
        //double usedRamMb = totalRamMb - availableRamMb;

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

    private static (double UsedMb, double TotalMb) GetMemoryUsage()
    {
        using var searcher =
            new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

        foreach (ManagementObject obj in searcher.Get())
        {
            double totalMb =
                Convert.ToDouble(obj["TotalVisibleMemorySize"]) / 1024;

            double freeMb =
                Convert.ToDouble(obj["FreePhysicalMemory"]) / 1024;

            double usedMb = totalMb - freeMb;

            return (usedMb, totalMb);
        }

        throw new InvalidOperationException(
            "Unable to retrieve memory information.");
    }
}