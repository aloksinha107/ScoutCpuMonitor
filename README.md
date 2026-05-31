# Scout Monitor

## Overview

Cross-platform-ready system monitoring console application built with .NET 8.

The application monitors:
- CPU usage
- RAM usage
- Disk usage

and supports a plugin architecture for extending behavior without modifying core logic.

The solution uses a simplified Clean Architecture approach to separate monitoring logic, application orchestration, and infrastructure concerns. Platform-specific monitoring is isolated behind the ISystemMonitor interface, allowing future Linux and macOS implementations to be added without modifying application logic. Dependency Injection is used throughout the application to improve extensibility and testability.

A plugin-based architecture was implemented using the IMonitorPlugin interface. This allows new integrations such as Slack notifications, email alerts, or database persistence to be added without changing the monitoring service. Two sample plugins were provided: a file logger plugin and a REST API publisher plugin.

## Architecture

The solution follows a simplified Clean Architecture approach:

- Domain
    - Models
    - Interfaces

- Application
    - Monitoring orchestration
    - Configuration

- Infrastructure
    - Windows monitoring implementation
    - Plugins

## Plugins

Implemented plugins:

- FileLoggerPlugin
- ApiPublisherPlugin

## Platform Support

The monitoring functionality is abstracted through `ISystemMonitor`.

This submission includes a Windows-specific implementation (`WindowsSystemMonitor`) using:

- PerformanceCounter
- WMI (System.Management)
- DriveInfo

Linux and macOS implementations can be added by implementing `ISystemMonitor`.

## Running

dotnet restore

dotnet build

dotnet run

## Configuration

Configuration is stored in:

appsettings.json

Settings:

- Monitoring interval
- API endpoint

## Future Improvements

- Dynamic plugin discovery
- Linux monitoring support
- macOS monitoring support
- Additional metrics
- Dashboard UI