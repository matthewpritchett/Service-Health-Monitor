# Service Health Monitor

A lightweight ASP.NET Core service for monitoring HTTP endpoints and network services, with a simple web UI and YAML-based configuration.

## Features

* Background monitoring using hosted services
* HTTP and ICMP (ping) health checks
* YAML configuration
* Simple in-memory status history
* Grouped services
* Extensible checker architecture
* Minimal ASP.NET Core application
* No external database required

## Supported Checks

* **HTTP** – Verify an endpoint responds with a specified status code
* **Ping** – Verify a host is reachable

Additional check types can be added by implementing `IHealthChecker`.

---

## Quick Start

1. Clone the repository.

2. Create a `servicemonitor.yaml` file:

```yaml
interval: 00:00:30

groups:
  - name: Websites
    services:
      - name: Google
        type: http
        url: https://www.google.com

  - name: Infrastructure
    services:
      - name: Router
        type: ping
        host: 192.168.1.1
```

3. Run the application:

```bash
dotnet run
```

4. Open your browser:

```
http://localhost:5130
```

The monitor will begin checking your configured services immediately and update the status page every 30 seconds.

---

## Configuration

Monitoring is configured in `servicemonitor.yaml`.

Example:

```yaml
interval: 00:00:30

groups:
  - name: Websites
    services:
      - name: Google
        type: http
        url: https://www.google.com

      - name: GitHub
        type: http
        url: https://github.com

  - name: Infrastructure
    services:
      - name: Router
        type: ping
        host: 192.168.1.1

      - name: NAS
        type: ping
        host: nas.local
```

## Configuration Format

Top-level properties:

| Property | Description |
|----------|-------------|
| `interval` | How often all checks are executed |
| `groups` | Collection of service groups |

Each group contains:

| Property | Description |
|----------|-------------|
| `name` | Display name |
| `services` | Services to monitor |

Each service contains:

| Property | Description |
|----------|-------------|
| `name` | Display name |
| `type` | `http` or `ping` |
| `url` | HTTP endpoint (HTTP only) |
| `host` | Hostname or IP address (Ping only) |

---

## Architecture

The monitor runs as an ASP.NET Core hosted service.

Every configured interval it:

1. Loads the latest YAML configuration
2. Iterates each group
3. Executes the appropriate health checker
4. Stores the result
5. Updates the status page

Health check implementations are discovered via dependency injection using `IHealthChecker`.

---

## Extending

New check types can be added by implementing:

```csharp
public interface IHealthChecker
{
    string Type { get; }

    Task<CheckResult> CheckAsync(
        string groupName,
        MonitorServiceConfiguration service);
}
```

Register your implementation:

```csharp
builder.Services.AddSingleton<IHealthChecker, MyCustomChecker>();
```

Then reference its `Type` value in the YAML configuration.

---

## Storage

The default implementation stores results in memory.

```csharp
builder.Services.AddSingleton<ICheckResultStore, InMemoryCheckResultStore>();
```

Alternative storage implementations can be created by implementing `ICheckResultStore`.
