# Pipelink.Insights

Pipelink.Insights, .NET uygulamalarınızda OpenTelemetry entegrasyonunu kolaylaştıran bir kütüphanedir. Uygulama performansını, hataları ve davranışları izlemenizi sağlar.

## Özellikler

- ASP.NET Core request/response izleme
- HTTP Client istek takibi
- Entity Framework Core sorgu izleme
- Redis operasyon takibi
- SQL Server sorgu izleme
- Özelleştirilebilir izleme yapılandırması
- OpenTelemetry Collector entegrasyonu
- Doğrudan OpenTelemetry yapılandırması desteği

## Kurulum

NuGet paketini projenize ekleyin:

```bash
dotnet add package Pipelink.Insights
```

## Kullanım

### 1. Standart ASP.NET Core Kullanımı

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

### 2. Doğrudan OpenTelemetry TracerProviderBuilder ile Kullanım

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

### 3. Özel OpenTelemetry Yapılandırması ile Kullanım

```csharp
services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        // Özel yapılandırmalar
        builder.AddSource("MySource");
        
        // Pipelink.Insights yapılandırması
        var options = new OpenTelemetryOptions
        {
            ServiceName = "MyService",
            ServiceVersion = "1.0.0",
            Endpoint = "http://localhost:4317"
        };
        builder.ConfigurePipelinkOpenTelemetry(options);
        
        // Ek yapılandırmalar
        builder.AddProcessor(new MyCustomProcessor());
    });
```

## Yapılandırma Seçenekleri

| Özellik | Açıklama | Varsayılan Değer |
|---------|-----------|------------------|
| ServiceName | İzlenen servisin adı | - |
| ServiceVersion | Servis versiyonu | "1.0.0" |
| Endpoint | OpenTelemetry Collector endpoint'i | "http://localhost:4317" |
| EnableAspNetCoreInstrumentation | ASP.NET Core izleme | true |
| HttpClientInstrumentation.Enabled | HTTP Client izleme | true |
| HttpClientInstrumentation.ExcludedUrls | İzlenmeyecek URL'ler | [] |
| HttpClientInstrumentation.RecordException | HTTP hatalarını kaydet | true |
| EnableEntityFrameworkCoreInstrumentation | EF Core izleme | false |
| EnableRedisInstrumentation | Redis izleme | false |
| EnableSqlClientInstrumentation | SQL Server izleme | false |

## Docker Compose Örneği

OpenTelemetry Collector ve Jaeger'ı başlatmak için:

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

OpenTelemetry Collector yapılandırması (`otel-collector-config.yaml`):

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

## Test Projesi

API projesi ile test etmek için:

1. Projeyi klonlayın:
```bash
git clone https://github.com/yourusername/pipelink.insights.git
cd pipelink.insights
```

2. Docker Compose ile gerekli servisleri başlatın:
```bash
docker-compose up -d
```

3. API projesini çalıştırın:
```bash
cd Pipelink.Insights.Api
dotnet run
```

4. Test endpointlerini çağırın:
```bash
# HTTP Client testi
curl http://localhost:5000/api/test/http

# Entity Framework Core testi
curl http://localhost:5000/api/test/ef

# Redis testi
curl http://localhost:5000/api/test/redis

# SQL Server testi
curl http://localhost:5000/api/test/sql
```

5. Jaeger UI'da izlemeleri görüntüleyin:
```
http://localhost:16686
```

## Lisans

MIT License 