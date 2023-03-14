using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StartFMS.Models;
using StartFMS_BackendAPI.Extensions;

namespace StartFMS_BackendAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly BackendContext _backendContext;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        BackendContext backendContext)
    {
        _logger = logger;
        _backendContext = backendContext;
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

    [HttpGet("AccountUsers")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public string GetAccountUsers() {
        UsersAuthorize users = new UsersAuthorize(Request);
        try {
            return JsonConvert.SerializeObject(_backendContext.A00AccountUsers.ToList());
        }
        catch (Exception ex) {
            return $"Error : {ex.Message}";
        }
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
    public string GetDBConfig(string name) {
        try {
            string? config = Config.GetConnectionString(name);
            return $"{name} : {config}";
        }
        catch (Exception ex) {
            return $"Error : {ex.Message}";
        }

    }
}
