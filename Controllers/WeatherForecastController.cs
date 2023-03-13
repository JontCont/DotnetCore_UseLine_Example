using Microsoft.AspNetCore.Mvc;
using start5M.Line.WebAPI.Extensions;

namespace dotnetCore_BackendAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("Config")]
    public string GetConfig(string name) {
        try {
            string? config = Config.GetConfiguration().GetValue<string>(name);
            return $"{name} : {config}";
        }catch (Exception ex) {
            return $"Error : {ex.Message}";
        }

    }

    [HttpGet("DBConfig")]
    public string GetDBConfig() {
        try {
            string? config = Config.GetConfiguration().GetValue<string>("ConnectionStrings:Default");
            return $"ConnectionStrings : {config}";
        }
        catch (Exception ex) {
            return $"Error : {ex.Message}";
        }

    }
}
