using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Pipelink.Insights.Options;

namespace Pipelink.Insights.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry in ASP.NET Core applications.
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry instrumentation to the service collection with custom configuration.
    /// </summary>
    /// <param name="services">The service collection to add OpenTelemetry to.</param>
    /// <param name="configure">An action to configure the OpenTelemetry options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPipelinkOpenTelemetry(
        this IServiceCollection services,
        Action<OpenTelemetryOptions> configure)
    {
        services.Configure(configure);
        
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                var options = services.BuildServiceProvider()
                    .GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;

                builder.ConfigurePipelinkOpenTelemetry(options);
            });

        return services;
    }

    /// <summary>
    /// Adds OpenTelemetry instrumentation to the service collection with simplified configuration.
    /// </summary>
    /// <param name="services">The service collection to add OpenTelemetry to.</param>
    /// <param name="serviceName">The name of the service being instrumented.</param>
    /// <param name="serviceVersion">The version of the service being instrumented.</param>
    /// <param name="endpoint">The endpoint URL for the OpenTelemetry collector.</param>
    /// <param name="enableAspNetCoreInstrumentation">Whether to enable ASP.NET Core instrumentation.</param>
    /// <param name="enableHttpClientInstrumentation">Whether to enable HttpClient instrumentation.</param>
    /// <param name="excludedHttpUrls">Array of URLs to exclude from HttpClient tracing.</param>
    /// <param name="recordHttpExceptions">Whether to record exceptions in HTTP requests.</param>
    /// <param name="enableEntityFrameworkCoreInstrumentation">Whether to enable Entity Framework Core instrumentation.</param>
    /// <param name="enableRedisInstrumentation">Whether to enable Redis instrumentation.</param>
    /// <param name="enableSqlClientInstrumentation">Whether to enable SQL Client instrumentation.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPipelinkOpenTelemetry(
        this IServiceCollection services,
        string serviceName,
        string serviceVersion = "1.0.0",
        string endpoint = "http://localhost:4317",
        bool enableAspNetCoreInstrumentation = true,
        bool enableHttpClientInstrumentation = true,
        string[]? excludedHttpUrls = null,
        bool recordHttpExceptions = true,
        bool enableEntityFrameworkCoreInstrumentation = false,
        bool enableRedisInstrumentation = false,
        bool enableSqlClientInstrumentation = false)
    {
        return services.AddPipelinkOpenTelemetry(options =>
        {
            options.ServiceName = serviceName;
            options.ServiceVersion = serviceVersion;
            options.Endpoint = endpoint;
            options.EnableAspNetCoreInstrumentation = enableAspNetCoreInstrumentation;
            options.HttpClientInstrumentation.Enabled = enableHttpClientInstrumentation;
            options.HttpClientInstrumentation.ExcludedUrls = excludedHttpUrls ?? Array.Empty<string>();
            options.HttpClientInstrumentation.RecordException = recordHttpExceptions;
            options.EnableEntityFrameworkCoreInstrumentation = enableEntityFrameworkCoreInstrumentation;
            options.EnableRedisInstrumentation = enableRedisInstrumentation;
            options.EnableSqlClientInstrumentation = enableSqlClientInstrumentation;
        });
    }

    /// <summary>
    /// Configures OpenTelemetry TracerProviderBuilder with Pipelink.Insights options.
    /// </summary>
    /// <param name="builder">The TracerProviderBuilder to configure.</param>
    /// <param name="options">The OpenTelemetry options to use for configuration.</param>
    /// <returns>The TracerProviderBuilder for chaining.</returns>
    public static TracerProviderBuilder ConfigurePipelinkOpenTelemetry(
        this TracerProviderBuilder builder,
        OpenTelemetryOptions options)
    {
        builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion));

        if (options.EnableAspNetCoreInstrumentation)
            builder.AddAspNetCoreInstrumentation();

        if (options.HttpClientInstrumentation.Enabled)
        {
            builder.AddHttpClientInstrumentation(httpOptions =>
            {
                if (options.HttpClientInstrumentation.ExcludedUrls.Any())
                {
                    httpOptions.FilterHttpRequestMessage = httpRequestMessage =>
                        !options.HttpClientInstrumentation.ExcludedUrls
                            .Any(url => httpRequestMessage.RequestUri?.ToString().Contains(url) == true);
                }
                httpOptions.RecordException = options.HttpClientInstrumentation.RecordException;
            });
        }

        if (options.EnableEntityFrameworkCoreInstrumentation)
            builder.AddEntityFrameworkCoreInstrumentation();

        if (options.EnableRedisInstrumentation)
            builder.AddRedisInstrumentation();

        if (options.EnableSqlClientInstrumentation)
            builder.AddSqlClientInstrumentation();

        builder.AddOtlpExporter(opts => opts.Endpoint = new Uri(options.Endpoint));

        return builder;
    }
} 