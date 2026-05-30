using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Domain.Models;

namespace ScoutMonitor.Infrastructure.Plugins;

public class ApiPublisherPlugin : IMonitorPlugin
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ApiPublisherPlugin(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task ProcessAsync(SystemMetrics metrics)
    {
        string endpoint = _configuration["Api:Endpoint"]!;

        var payload = new
        {
            cpu = metrics.CpuUsagePercent,
            ram_used = metrics.RamUsedMb,
            disk_used = metrics.DiskUsedGb
        };

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
    }
}