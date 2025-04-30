using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pipelink.Insights.Api.Data;
using Pipelink.Insights.Api.Models;
using StackExchange.Redis;
using System.Data.SqlClient;

namespace Pipelink.Insights.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;

    public TestController(
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext dbContext,
        IConnectionMultiplexer redis,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _redis = redis;
        _configuration = configuration;
    }

    [HttpGet("http")]
    public async Task<IActionResult> TestHttp()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://httpbin.org/get");
        return Ok(await response.Content.ReadAsStringAsync());
    }

    [HttpGet("http/error")]
    public async Task<IActionResult> TestHttpError()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://httpbin.org/status/500");
        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("http/timeout")]
    public async Task<IActionResult> TestHttpTimeout()
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(1);
        var response = await client.GetAsync("https://httpbin.org/delay/2");
        return Ok(await response.Content.ReadAsStringAsync());
    }

    [HttpGet("ef")]
    public async Task<IActionResult> TestEf()
    {
        var forecasts = await _dbContext.WeatherForecasts.ToListAsync();
        return Ok(forecasts);
    }

    [HttpGet("ef/complex")]
    public async Task<IActionResult> TestEfComplex()
    {
        var forecasts = await _dbContext.WeatherForecasts
            .Where(f => f.TemperatureC > 0)
            .OrderByDescending(f => f.Date)
            .Take(5)
            .ToListAsync();
        return Ok(forecasts);
    }

    [HttpGet("ef/error")]
    public async Task<IActionResult> TestEfError()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("SELECT * FROM NonExistentTable");
        return Ok();
    }

    [HttpGet("redis")]
    public async Task<IActionResult> TestRedis()
    {
        var db = _redis.GetDatabase();
        await db.StringSetAsync("test_key", "test_value");
        var value = await db.StringGetAsync("test_key");
        return Ok(value.ToString());
    }

    [HttpGet("redis/error")]
    public async Task<IActionResult> TestRedisError()
    {
        var db = _redis.GetDatabase();
        await db.ExecuteAsync("INVALID_COMMAND");
        return Ok();
    }

    [HttpGet("sql")]
    public async Task<IActionResult> TestSql()
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        using var command = new SqlCommand("SELECT * FROM WeatherForecasts", connection);
        using var reader = await command.ExecuteReaderAsync();
        var forecasts = new List<WeatherForecast>();
        while (await reader.ReadAsync())
        {
            forecasts.Add(new WeatherForecast(
                DateOnly.FromDateTime(reader.GetDateTime(1)),
                reader.GetInt32(2),
                reader.GetString(3)
            ));
        }
        return Ok(forecasts);
    }

    [HttpGet("sql/complex")]
    public async Task<IActionResult> TestSqlComplex()
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        using var command = new SqlCommand(@"
            SELECT * FROM WeatherForecasts 
            WHERE TemperatureC > 0 
            ORDER BY Date DESC 
            OFFSET 0 ROWS 
            FETCH NEXT 5 ROWS ONLY", connection);
        using var reader = await command.ExecuteReaderAsync();
        var forecasts = new List<WeatherForecast>();
        while (await reader.ReadAsync())
        {
            forecasts.Add(new WeatherForecast(
                DateOnly.FromDateTime(reader.GetDateTime(1)),
                reader.GetInt32(2),
                reader.GetString(3)
            ));
        }
        return Ok(forecasts);
    }

    [HttpGet("sql/error")]
    public async Task<IActionResult> TestSqlError()
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        using var command = new SqlCommand("SELECT * FROM NonExistentTable", connection);
        await command.ExecuteReaderAsync();
        return Ok();
    }
} 