# Pipelink.Insights.Api

Pipelink.Insights.Api, OpenTelemetry entegrasyonunu test etmek ve göstermek için oluşturulmuş bir örnek API projesidir.

## Özellikler

- ASP.NET Core Web API
- OpenTelemetry entegrasyonu
- HTTP Client örnekleri
- Entity Framework Core örnekleri
- Redis örnekleri
- SQL Server örnekleri
- Swagger/OpenAPI desteği

## Gereksinimler

- .NET 8.0 SDK
- Docker ve Docker Compose
- SQL Server (Docker ile sağlanabilir)
- Redis (Docker ile sağlanabilir)

## Kurulum

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

## API Endpointleri

### HTTP Client Testleri

- `GET /api/test/http` - Basit HTTP isteği
- `GET /api/test/http/error` - Hata durumu testi
- `GET /api/test/http/timeout` - Timeout durumu testi

### Entity Framework Core Testleri

- `GET /api/test/ef` - Basit EF Core sorgusu
- `GET /api/test/ef/complex` - Karmaşık EF Core sorgusu
- `GET /api/test/ef/error` - EF Core hata durumu

### Redis Testleri

- `GET /api/test/redis` - Basit Redis operasyonu
- `GET /api/test/redis/error` - Redis hata durumu

### SQL Server Testleri

- `GET /api/test/sql` - Basit SQL sorgusu
- `GET /api/test/sql/complex` - Karmaşık SQL sorgusu
- `GET /api/test/sql/error` - SQL hata durumu

## OpenTelemetry Konfigürasyonu

API projesi, Pipelink.Insights kütüphanesini kullanarak OpenTelemetry entegrasyonunu yapılandırır:

```csharp
services.AddPipelinkOpenTelemetry(options =>
{
    options.ServiceName = "Pipelink.Insights.Api";
    options.ServiceVersion = "1.0.0";
    options.Endpoint = "http://localhost:4317";
    
    // Tüm izleme özelliklerini etkinleştir
    options.EnableAspNetCoreInstrumentation = true;
    options.EnableEntityFrameworkCoreInstrumentation = true;
    options.EnableRedisInstrumentation = true;
    options.EnableSqlClientInstrumentation = true;
    
    // HTTP Client izleme ayarları
    options.HttpClientInstrumentation.Enabled = true;
    options.HttpClientInstrumentation.ExcludedUrls = new[] { "health", "metrics" };
    options.HttpClientInstrumentation.RecordException = true;
});
```

## Docker Compose Yapılandırması

Proje, aşağıdaki servisleri içeren bir Docker Compose yapılandırması ile gelir:

- OpenTelemetry Collector
- Jaeger (Trace görselleştirme)
- SQL Server
- Redis

Docker Compose dosyası (`docker-compose.yml`):

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

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Passw0rd
    ports:
      - "1433:1433"

  redis:
    image: redis:latest
    ports:
      - "6379:6379"
```

## Test Senaryoları

### 1. HTTP Client Testi

```bash
curl http://localhost:5000/api/test/http
```

### 2. Entity Framework Core Testi

```bash
curl http://localhost:5000/api/test/ef
```

### 3. Redis Testi

```bash
curl http://localhost:5000/api/test/redis
```

### 4. SQL Server Testi

```bash
curl http://localhost:5000/api/test/sql
```

## İzleme Görselleştirme

Jaeger UI'ına erişmek için:
```
http://localhost:16686
```

## Lisans

MIT License 