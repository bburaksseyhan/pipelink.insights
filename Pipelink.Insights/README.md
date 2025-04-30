# Pipelink.Insights

Pipelink.Insights is a library that facilitates OpenTelemetry integration for your .NET applications. This library enables you to monitor your application's performance, errors, and behavior.

## Features

- ASP.NET Core request/response monitoring
- HTTP Client request monitoring
- Entity Framework Core query monitoring
- Redis operation monitoring
- SQL Server query monitoring
- Customizable monitoring configuration
- OpenTelemetry Collector integration

## Installation

```bash
dotnet add package Pipelink.Insights
```

## Usage

### Basic Usage

```csharp
services.AddPipelinkOpenTelemetry("MyService");
```

### Advanced Configuration

```csharp
services.AddPipelinkOpenTelemetry(options =>
{
    options.ServiceName = "MyService";
    options.ServiceVersion = "1.0.0";
    options.Endpoint = "http://localhost:4317";
    
    // ASP.NET Core monitoring
    options.EnableAspNetCoreInstrumentation = true;
    
    // HTTP Client monitoring
    options.HttpClientInstrumentation.Enabled = true;
    options.HttpClientInstrumentation.ExcludedUrls = new[] { "health", "metrics" };
    options.HttpClientInstrumentation.RecordException = true;
    
    // Entity Framework Core monitoring
    options.EnableEntityFrameworkCoreInstrumentation = true;
    
    // Redis monitoring
    options.EnableRedisInstrumentation = true;
    
    // SQL Server monitoring
    options.EnableSqlClientInstrumentation = true;
});
```

### Simplified Configuration

```csharp
services.AddPipelinkOpenTelemetry(
    serviceName: "MyService",
    serviceVersion: "1.0.0",
    endpoint: "http://localhost:4317",
    enableAspNetCoreInstrumentation: true,
    enableHttpClientInstrumentation: true,
    excludedHttpUrls: new[] { "health", "metrics" },
    recordHttpExceptions: true,
    enableEntityFrameworkCoreInstrumentation: true,
    enableRedisInstrumentation: true,
    enableSqlClientInstrumentation: true
);
```

## Configuration Options

### OpenTelemetryOptions

| Property | Description | Default |
|----------|-------------|---------|
| ServiceName | Service name | - |
| ServiceVersion | Service version | "1.0.0" |
| Endpoint | OpenTelemetry Collector endpoint | "http://localhost:4317" |
| EnableAspNetCoreInstrumentation | ASP.NET Core monitoring | true |
| EnableEntityFrameworkCoreInstrumentation | EF Core monitoring | false |
| EnableRedisInstrumentation | Redis monitoring | false |
| EnableSqlClientInstrumentation | SQL Server monitoring | false |

### HttpClientInstrumentationOptions

| Property | Description | Default |
|----------|-------------|---------|
| Enabled | HTTP Client monitoring enabled/disabled | true |
| ExcludedUrls | URLs to exclude from monitoring | [] |
| RecordException | Monitor HTTP errors | true |

## OpenTelemetry Collector with Docker Compose

```yaml
version: '3'
services:
  otel-collector:
    image: otel/opentelemetry-collector:latest
    command: ["--config=/etc/otel-collector-config.yaml"]
    volumes:
      - ./otel-collector-config.yaml:/etc/otel-collector-config.yaml
    ports:
      - "4317:4317"   # OTLP gRPC
      - "4318:4318"   # OTLP HTTP
      - "8888:8888"   # Prometheus metrics
      - "8889:8889"   # Prometheus exporter metrics
      - "13133:13133" # Health check
      - "55679:55679" # ZPages

  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "16686:16686" # UI
      - "14250:14250" # Model
```

## OpenTelemetry Collector Configuration

```yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318

processors:
  batch:
    timeout: 1s
    send_batch_size: 1024

exporters:
  jaeger:
    endpoint: jaeger:14250
    tls:
      insecure: true

service:
  pipelines:
    traces:
      receivers: [otlp]
      processors: [batch]
      exporters: [jaeger]
```

## License

MIT License 