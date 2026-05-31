# Scout Monitor

## Overview

Scout Monitor is a .NET 8 console application that monitors system resources and publishes monitoring updates through a plugin-based architecture.

The application periodically collects:

- CPU Usage (%)
- RAM Usage (Used / Total)
- Disk Usage (Used / Total)

and then:

- Displays the metrics in the console
- Logs the metrics to a local file
- Sends the metrics to a configurable REST API endpoint

The solution is designed to be extensible through plugins and follows a simplified Clean Architecture approach with dependency injection.

---

## Architecture

The project is organized into three logical layers:

### Domain

Contains core models and contracts.

- `SystemMetrics`
- `ISystemMonitor`
- `IMonitorPlugin`

### Application

Contains orchestration and configuration logic.

- `MonitoringService`
- `MonitoringSettings`
- `ApiSettings`

### Infrastructure

Contains platform-specific and external integrations.

- `WindowsSystemMonitor`
- `FileLoggerPlugin`
- `ApiPublisherPlugin`

---

## Design Approach

The primary design goal was separation of concerns and extensibility.

### Monitoring Abstraction

System metric collection is abstracted behind the `ISystemMonitor` interface.

This allows additional implementations such as:

- LinuxSystemMonitor
- MacSystemMonitor

to be added without modifying application logic.

### Plugin Architecture

Plugins implement the `IMonitorPlugin` interface.

The monitoring service is unaware of specific plugin implementations and simply executes all registered plugins for each monitoring cycle.

This follows the Open/Closed Principle by allowing new functionality to be added without modifying existing monitoring logic.

### Dependency Injection

Dependency Injection is used throughout the application to:

- Decouple components
- Improve maintainability
- Simplify future testing
- Support plugin extensibility

---

## Implemented Plugins

### FileLoggerPlugin

Writes monitoring information to a local file:

```text
metrics.log
```

### ApiPublisherPlugin

Posts monitoring data to a configurable REST endpoint.

Example payload:

```json
{
  "cpu": 12.5,
  "ram_used": 8123.4,
  "disk_used": 215347.0
}
```

---

## Assumptions

The following assumptions were made while implementing the solution:

- Full monitoring support was implemented for Windows only.
- Cross-platform support is achieved through abstraction (`ISystemMonitor`) rather than implementing all platform-specific monitors.
- The application monitors the first available fixed disk drive on the system.

---

## Configuration

Configuration is stored in:

```text
appsettings.json
```

Example:

```json
{
  "Monitoring": {
    "IntervalSeconds": 5
  },
  "Api": {
    "Endpoint": "https://httpbin.org/post"
  }
}
```

### Available Settings

| Setting | Description |
|----------|-------------|
| IntervalSeconds | Monitoring interval in seconds |
| Endpoint | REST API endpoint used by ApiPublisherPlugin |

---

## Prerequisites

- .NET 8 SDK
- Windows Operating System

Verify installation:

```bash
dotnet --version
```

---

## How To Build

Restore dependencies:

```bash
dotnet restore
```

Build the application:

```bash
dotnet build
```

---

## How To Run

Run the application:

```bash
dotnet run
```

The application will begin displaying monitoring information every configured interval.

Press:

```text
Ctrl + C
```

to stop the application gracefully.

---

## Output Examples

### Console Output

```text
========================================
Timestamp : 2026-05-31 10:00:00
CPU       : 12.50%
RAM       : 8123.44 MB / 16384.00 MB
DISK      : 215.20 GB / 476.94 GB
========================================
```

### File Output

```text
[2026-05-31 10:00:00] CPU=12.50% RAM=8123.44/16384.00 MB DISK=215.20/476.94 GB
```

---

## Future Improvements

Potential enhancements include:

- Dynamic plugin discovery and loading
- Linux monitoring implementation
- macOS monitoring implementation
- Additional system metrics (network, processes, etc.)
- Dashboard or UI layer
- Retry and resiliency policies for API communication

---

## Challenges Encountered

The primary challenge was implementing platform-specific monitoring while keeping the application architecture platform-independent.

This was addressed by isolating operating-system-specific logic behind the `ISystemMonitor` abstraction, allowing additional platform implementations to be introduced without modifying application-level code.