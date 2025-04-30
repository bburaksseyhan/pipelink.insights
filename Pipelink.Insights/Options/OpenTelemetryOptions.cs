namespace Pipelink.Insights.Options;

/// <summary>
/// Configuration options for HttpClient instrumentation in OpenTelemetry.
/// </summary>
public class HttpClientInstrumentationOptions
{
    /// <summary>
    /// Gets or sets whether HttpClient instrumentation is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets an array of URLs to exclude from tracing.
    /// Any HTTP request containing these URLs in its path will not be traced.
    /// </summary>
    public string[] ExcludedUrls { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets whether to record exceptions in HTTP requests.
    /// When true, HTTP exceptions will be included in the trace data.
    /// </summary>
    public bool RecordException { get; set; } = true;
}

/// <summary>
/// Configuration options for OpenTelemetry instrumentation.
/// </summary>
public class OpenTelemetryOptions
{
    /// <summary>
    /// Gets or sets the name of the service being instrumented.
    /// This name will be used to identify the service in telemetry data.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the service being instrumented.
    /// This version will be included in telemetry data.
    /// </summary>
    public string ServiceVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the endpoint URL for the OpenTelemetry collector.
    /// This is where telemetry data will be sent.
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:4317";

    /// <summary>
    /// Gets or sets whether ASP.NET Core instrumentation is enabled.
    /// When true, ASP.NET Core requests will be traced.
    /// </summary>
    public bool EnableAspNetCoreInstrumentation { get; set; } = true;

    /// <summary>
    /// Gets or sets the configuration for HttpClient instrumentation.
    /// </summary>
    public HttpClientInstrumentationOptions HttpClientInstrumentation { get; set; } = new();

    /// <summary>
    /// Gets or sets whether Entity Framework Core instrumentation is enabled.
    /// When true, database operations will be traced.
    /// </summary>
    public bool EnableEntityFrameworkCoreInstrumentation { get; set; } = false;

    /// <summary>
    /// Gets or sets whether Redis instrumentation is enabled.
    /// When true, Redis operations will be traced.
    /// </summary>
    public bool EnableRedisInstrumentation { get; set; } = false;

    /// <summary>
    /// Gets or sets whether SQL Client instrumentation is enabled.
    /// When true, direct SQL operations will be traced.
    /// </summary>
    public bool EnableSqlClientInstrumentation { get; set; } = false;
} 