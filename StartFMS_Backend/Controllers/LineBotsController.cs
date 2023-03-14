using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StartFMS_BackendAPI.Line.WebAPI.Extensions.LineBots;
namespace StartFMS_BackendAPI.Controllers;

[ApiController]
[Route("/LineApi/")]
public class LineBotsController : ControllerBase
{
    private readonly ILogger<LineBotsController> _logger;
    private LineBots _lineBots;

    public LineBotsController(ILogger<LineBotsController> logger, LineBots lineBots)
    {
        _logger = logger;
        _lineBots = lineBots;
    }

    [HttpPost("", Name = "Message Reply")]
    public async Task<string> Post() {
        try {
            LineBots line = await _lineBots.LoadAsync(Request.Body);
            await line.MessageAsync();
            return JsonConvert.SerializeObject(new {
                Success = true,
                Message = "",
            });
        }
        catch (Exception ex) {
            return JsonConvert.SerializeObject(new {
                Success = false,
                Message = ex.Message,
            });
        }
    }


}
