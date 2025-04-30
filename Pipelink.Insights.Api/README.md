# Pipelink.Insights.Api

Pipelink.Insights.Api is a sample API project created to test and demonstrate OpenTelemetry integration.

## Features

- ASP.NET Core Web API
- OpenTelemetry integration
- HTTP Client examples
- Entity Framework Core examples
- Redis examples
- SQL Server examples
- Swagger/OpenAPI support

## Requirements

- .NET 8.0 SDK
- Docker and Docker Compose
- SQL Server (can be provided via Docker)
- Redis (can be provided via Docker)

## Installation

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

## API Endpoints

### HTTP Client Tests

- `GET /api/test/http` - Simple HTTP request
- `GET /api/test/http/error` - Error state test
- `GET /api/test/http/timeout` - Timeout state test

### Entity Framework Core Tests

- `GET /api/test/ef` - Simple EF Core query
- `GET /api/test/ef/complex` - Complex EF Core query
- `GET /api/test/ef/error` - EF Core error state

### Redis Tests

- `GET /api/test/redis` - Simple Redis operation
- `GET /api/test/redis/error` - Redis error state

### SQL Server Tests

- `GET /api/test/sql` - Simple SQL query
- `GET /api/test/sql/complex` - Complex SQL query
- `GET /api/test/sql/error` - SQL error state

## OpenTelemetry Configuration

The API project configures OpenTelemetry integration using the Pipelink.Insights library:

```csharp
services.AddPipelinkOpenTelemetry(options =>
{
    options.ServiceName = "Pipelink.Insights.Api";
    options.ServiceVersion = "1.0.0";
    options.Endpoint = "http://localhost:4317";
    
    // Enable all monitoring features
    options.EnableAspNetCoreInstrumentation = true;
    options.EnableEntityFrameworkCoreInstrumentation = true;
    options.EnableRedisInstrumentation = true;
    options.EnableSqlClientInstrumentation = true;
    
    // HTTP Client monitoring settings
    options.HttpClientInstrumentation.Enabled = true;
    options.HttpClientInstrumentation.ExcludedUrls = new[] { "health", "metrics" };
    options.HttpClientInstrumentation.RecordException = true;
});
```

## Docker Compose Configuration

The project comes with a Docker Compose configuration that includes the following services:

- OpenTelemetry Collector
- Jaeger (Trace visualization)
- SQL Server
- Redis

Docker Compose file (`docker-compose.yml`):

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

## Test Scenarios

### 1. HTTP Client Test

```bash
curl http://localhost:5000/api/test/http
```

### 2. Entity Framework Core Test

```bash
curl http://localhost:5000/api/test/ef
```

### 3. Redis Test

```bash
curl http://localhost:5000/api/test/redis
```

### 4. SQL Server Test

```bash
curl http://localhost:5000/api/test/sql
```

## Trace Visualization

To access Jaeger UI:
```
http://localhost:16686
```

## License

MIT License 