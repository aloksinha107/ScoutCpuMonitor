using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScoutMonitor.Application.Configuration;
using ScoutMonitor.Application;
using ScoutMonitor.Domain.Interfaces;
using ScoutMonitor.Infrastructure.Monitoring;
using ScoutMonitor.Infrastructure.Plugins;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true);

builder.Services.Configure<MonitoringSettings>(
    builder.Configuration.GetSection("Monitoring"));

builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("Api"));

builder.Services.AddSingleton<ISystemMonitor, WindowsSystemMonitor>();

builder.Services.AddSingleton<IMonitorPlugin, FileLoggerPlugin>();

builder.Services.AddHttpClient<ApiPublisherPlugin>();

builder.Services.AddSingleton<IMonitorPlugin>(provider =>
    provider.GetRequiredService<ApiPublisherPlugin>());

builder.Services.AddSingleton<MonitoringService>();

var host = builder.Build();

var monitoringService =
    host.Services.GetRequiredService<MonitoringService>();

using var cancellationTokenSource =
    new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;

    Console.WriteLine();
    Console.WriteLine("Stopping monitor...");

    cancellationTokenSource.Cancel();
};

await monitoringService.RunAsync(
    cancellationTokenSource.Token);