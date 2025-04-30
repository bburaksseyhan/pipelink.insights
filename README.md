# Pipelink.Insights

Pipelink.Insights is a library that facilitates OpenTelemetry integration in your .NET applications. It enables you to monitor application performance, errors, and behaviors.

## Features

- ASP.NET Core request/response monitoring
- HTTP Client request tracking
- Entity Framework Core query monitoring
- Redis operation tracking
- SQL Server query monitoring
- Customizable monitoring configuration
- OpenTelemetry Collector integration
- Direct OpenTelemetry configuration support

## Installation

Add the NuGet package to your project:

```bash
dotnet add package Pipelink.Insights
```

## Usage

### 1. Standard ASP.NET Core Usage

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

### 2. Usage with Direct OpenTelemetry TracerProviderBuilder

```csharp
var options = new OpenTelemetryOptions
{
    ServiceName = "MyService",
    ServiceVersion = "1.0.0",
    Endpoint = "http://localhost:4317",
    EnableAspNetCoreInstrumentation = true,
    HttpClientInstrumentation = new HttpClientInstrumentationOptions
    {
        Enabled = true,
        ExcludedUrls = new[] { "health", "metrics" },
        RecordException = true
    }
};

builder.ConfigurePipelinkOpenTelemetry(options);
```

### 3. Usage with Custom OpenTelemetry Configuration

```csharp
services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        // Custom configurations
        builder.AddSource("MySource");
        
        // Pipelink.Insights configuration
        var options = new OpenTelemetryOptions
        {
            ServiceName = "MyService",
            ServiceVersion = "1.0.0",
            Endpoint = "http://localhost:4317"
        };
        builder.ConfigurePipelinkOpenTelemetry(options);
        
        // Additional configurations
        builder.AddProcessor(new MyCustomProcessor());
    });
```

## Configuration Options

| Property | Description | Default Value |
|----------|-------------|---------------|
| ServiceName | Name of the monitored service | - |
| ServiceVersion | Service version | "1.0.0" |
| Endpoint | OpenTelemetry Collector endpoint | "http://localhost:4317" |
| EnableAspNetCoreInstrumentation | ASP.NET Core monitoring | true |
| HttpClientInstrumentation.Enabled | HTTP Client monitoring | true |
| HttpClientInstrumentation.ExcludedUrls | URLs to exclude from monitoring | [] |
| HttpClientInstrumentation.RecordException | Record HTTP errors | true |
| EnableEntityFrameworkCoreInstrumentation | EF Core monitoring | false |
| EnableRedisInstrumentation | Redis monitoring | false |
| EnableSqlClientInstrumentation | SQL Server monitoring | false |

## Docker Compose Example

To start OpenTelemetry Collector and Jaeger:

```yaml
version: '3'
services:
  otel-collector:
    image: otel/opentelemetry-collector:latest
    command: ["--config=/etc/otel-collector-config.yaml"]
    volumes:
      - ./otel-collector-config.yaml:/etc/otel-collector-config.yaml
    ports:
      - "4317:4317"
      - "4318:4318"

  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "16686:16686"
```

OpenTelemetry Collector configuration (`otel-collector-config.yaml`):

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

## Test Project

To test with the API project:

1. Clone the project:
```bash
git clone https://github.com/yourusername/pipelink.insights.git
cd pipelink.insights
```

2. Start required services with Docker Compose:
```bash
docker-compose up -d
```

3. Run the API project:
```bash
cd Pipelink.Insights.Api
dotnet run
```

4. Call test endpoints:
```bash
# HTTP Client test
curl http://localhost:5000/api/test/http

# Entity Framework Core test
curl http://localhost:5000/api/test/ef

# Redis test
curl http://localhost:5000/api/test/redis

# SQL Server test
curl http://localhost:5000/api/test/sql
```

5. View traces in Jaeger UI:
```
http://localhost:16686
```

## License

MIT License 