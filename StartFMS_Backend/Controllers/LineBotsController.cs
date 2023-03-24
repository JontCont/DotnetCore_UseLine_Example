using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StartFMS.Models.Backend;
using StartFMS_BackendAPI.Line.WebAPI.Extensions.LineBots;
using StartFMS.Extensions.Data;
using isRock.LineBot;
using static System.Net.Mime.MediaTypeNames;

namespace StartFMS_BackendAPI.Controllers;

[ApiController]
[Route("/LineApi/")]
public class LineBotsController : ControllerBase {
    private readonly ILogger<LineBotsController> _logger;
    private LineBots _lineBots;
    private readonly A00_BackendContext _backendContext;


    public LineBotsController(
        ILogger<LineBotsController> logger,
        LineBots lineBots,
        A00_BackendContext backendContext) {
        _logger = logger;
        _lineBots = lineBots;
        _backendContext = backendContext;
    }

    [HttpPost("", Name = "Message Reply")]
    public async Task<string> Post() {
        try {
            LineBots line = await _lineBots.LoadAsync(Request.Body);
            var LineEvent = line.ReceivedMessage.events.FirstOrDefault();
            string userId = LineEvent.source.userId;
            var lineOption = _backendContext.B10LineMessageOptions.Where(x => x.UserId == userId);
            TextMessage textMessage = new("");
            B10LineMessageOption b10Line = new B10LineMessageOption();
            switch (LineEvent.message.text) {
                case "目錄":
                    textMessage = new TextMessage($"請選擇下方選項 (請使用手機選擇)");
                    if(!lineOption.Any() || lineOption.Where(x => x.Type == "ChatGPT" && x.IsUse.ToLower() == "false").Any()) {
                        textMessage.quickReply.items.Add(new QuickReplyMessageAction("開啟 ChatGPT 聊天", "開啟 ChatGPT 聊天"));
                    }
                    else {
                        textMessage.quickReply.items.Add(new QuickReplyMessageAction("關閉 ChatGPT 聊天", "關閉 ChatGPT 聊天"));
                    }

                    if (!lineOption.Any() || lineOption.Where(x => x.Type == "Reply" && x.IsUse.ToLower() == "false").Any()) {
                        textMessage.quickReply.items.Add(new QuickReplyMessageAction("開啟 模仿說話", "開啟 模仿說話"));
                    }
                    else {
                        textMessage.quickReply.items.Add(new QuickReplyMessageAction("關閉 模仿說話", "關閉 模仿說話"));
                    }
                    line.ReplyUserID = LineEvent.replyToken;
                    line.PushMessage(textMessage);
                    break;
                case "開啟 ChatGPT 聊天":
                    if (!lineOption.Any()) {
                        _backendContext.B10LineMessageOptions.Add(new B10LineMessageOption { UserId = userId, IsUse = true.ToString(), Type = "ChatGPT" });
                    }
                    else {
                        b10Line = lineOption.Where(x => x.Type == "ChatGPT").FirstOrDefault();
                        b10Line.IsUse = true.ToString();
                        _backendContext.B10LineMessageOptions.Update(b10Line);
                    }
                    await _backendContext.SaveChangesAsync();
                    break;
                case "開啟 模仿說話":
                    if (!lineOption.Any()) {
                        _backendContext.B10LineMessageOptions.Add(new B10LineMessageOption { UserId = userId, IsUse = true.ToString(), Type = "Reply" });
                    }
                    else {
                        b10Line = lineOption.Where(x => x.Type == "Reply").FirstOrDefault();
                        b10Line.IsUse = true.ToString();
                        _backendContext.B10LineMessageOptions.Update(b10Line);
                    }
                    await _backendContext.SaveChangesAsync();
                    break;
                case "關閉 ChatGPT 聊天":
                    b10Line = lineOption.Where(x => x.Type == "ChatGPT").FirstOrDefault();
                    b10Line.IsUse = false.ToString();
                    _backendContext.B10LineMessageOptions.Update(b10Line);
                    await _backendContext.SaveChangesAsync();
                    break;
                case "關閉 模仿說話":
                    b10Line = lineOption.Where(x => x.Type == "Reply").FirstOrDefault();
                    b10Line.IsUse = false.ToString();
                    _backendContext.B10LineMessageOptions.Update(b10Line);
                    await _backendContext.SaveChangesAsync();
                    break;
                default:
                    line.ReplyUserID = LineEvent.replyToken;
                    if (lineOption.Where(x => x.Type == "Reply" && x.IsUse.ToLower() == "true").Any()) {
                        textMessage = new TextMessage($"您說的是 : {LineEvent.message.text}");
                        line.ReplyMessage(textMessage.text);
                    }
                    if (lineOption.Where(x => x.Type == "ChatGPT" && x.IsUse.ToLower() == "true").Any()) {
                        string Prompt = LineEvent.message.text.ToString().Replace("!chat ", "");
                        string Request = await Line.WebAPI.Extensions.ChatGPT.Chat.ResponseMessageAsync(Prompt);
                        textMessage = new TextMessage(Request);
                        line.ReplyMessage(textMessage.text);
                    }
                    break;
            }

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
