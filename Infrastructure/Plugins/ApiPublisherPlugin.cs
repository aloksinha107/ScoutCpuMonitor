using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ScoutMonitor.Application.Configuration;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Infrastructure.Plugins;

public class ApiPublisherPlugin : IMonitorPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _settings;

    public ApiPublisherPlugin(HttpClient httpClient, IOptions<ApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task ProcessAsync(SystemMetrics metrics)
    {
        var payload = new
        {
            cpu = metrics.CpuUsagePercent,
            ram_used = metrics.RamUsedMb,
            disk_used = metrics.DiskUsedGb * 1024
        };

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response =
                    await _httpClient.PostAsync(
                        _settings.Endpoint,
                        content);        

        response.EnsureSuccessStatusCode();
    }
}