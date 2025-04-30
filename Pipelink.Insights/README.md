# Pipelink.Insights

Pipelink.Insights, .NET uygulamalarınız için OpenTelemetry entegrasyonunu kolaylaştıran bir kütüphanedir. Bu kütüphane, uygulamanızın performansını, hatalarını ve davranışını izlemenizi sağlar.

## Özellikler

- ASP.NET Core request/response izleme
- HTTP Client isteklerinin izlenmesi
- Entity Framework Core sorgu izleme
- Redis operasyonlarının izlenmesi
- SQL Server sorgu izleme
- Özelleştirilebilir izleme konfigürasyonu
- OpenTelemetry Collector entegrasyonu

## Kurulum

```bash
dotnet add package Pipelink.Insights
```

## Kullanım

### Temel Kullanım

```csharp
services.AddPipelinkOpenTelemetry("MyService");
```

### Gelişmiş Konfigürasyon

```csharp
services.AddPipelinkOpenTelemetry(options =>
{
    options.ServiceName = "MyService";
    options.ServiceVersion = "1.0.0";
    options.Endpoint = "http://localhost:4317";
    
    // ASP.NET Core izleme
    options.EnableAspNetCoreInstrumentation = true;
    
    // HTTP Client izleme
    options.HttpClientInstrumentation.Enabled = true;
    options.HttpClientInstrumentation.ExcludedUrls = new[] { "health", "metrics" };
    options.HttpClientInstrumentation.RecordException = true;
    
    // Entity Framework Core izleme
    options.EnableEntityFrameworkCoreInstrumentation = true;
    
    // Redis izleme
    options.EnableRedisInstrumentation = true;
    
    // SQL Server izleme
    options.EnableSqlClientInstrumentation = true;
});
```

### Basitleştirilmiş Konfigürasyon

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

## Konfigürasyon Seçenekleri

### OpenTelemetryOptions

| Özellik | Açıklama | Varsayılan |
|---------|-----------|------------|
| ServiceName | Servis adı | - |
| ServiceVersion | Servis versiyonu | "1.0.0" |
| Endpoint | OpenTelemetry Collector endpoint'i | "http://localhost:4317" |
| EnableAspNetCoreInstrumentation | ASP.NET Core izleme | true |
| EnableEntityFrameworkCoreInstrumentation | EF Core izleme | false |
| EnableRedisInstrumentation | Redis izleme | false |
| EnableSqlClientInstrumentation | SQL Server izleme | false |

### HttpClientInstrumentationOptions

| Özellik | Açıklama | Varsayılan |
|---------|-----------|------------|
| Enabled | HTTP Client izleme aktif/pasif | true |
| ExcludedUrls | İzlenmeyecek URL'ler | [] |
| RecordException | HTTP hatalarını izle | true |

## Docker Compose ile OpenTelemetry Collector

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

## OpenTelemetry Collector Konfigürasyonu

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

## Lisans

MIT License 