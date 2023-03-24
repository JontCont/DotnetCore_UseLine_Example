using isRock.LineBot;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace StartFMS_BackendAPI.Line.WebAPI.Extensions.LineBots;

public static class LineBotsMessage {

    /// <summary>
    /// Line Bots 留言
    /// </summary>
    /// <param name="bots"></param>
    /// <returns></returns>
    public static async Task<string> MessageAsync(this LineBots bots) {
        // 確認 Message 是否存在
        try {
            var LineEvent = bots.ReceivedMessage.events.FirstOrDefault();
            bots.ReplyUserID = LineEvent.replyToken; // 設定回覆對象

            await bots.ReplyBotsMessage(LineEvent);
            return JsonConvert.SerializeObject(new { success = true, message = "" });
        }
        catch {
            return JsonConvert.SerializeObject(new {
                success = false,
                message = "error : message empty or not found event  "
            });
        }
    }

    /// <summary>
    /// Client 回應事件
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="lineEvent"></param>
    private static async Task ReplyBotsMessage(this LineBots bots, Event lineEvent) {
        TextMessage textMessage = new("");
        switch (lineEvent.type) {
            case "join":
                textMessage = new TextMessage($"大家好啊~");
                break;

            case "message":
                string text = lineEvent.message.text;
                if (string.IsNullOrEmpty(text)) break;

                if (text.Length > 7 && text.Substring(0, 7).Equals("!reply ")) {
                    textMessage = new TextMessage($"您說的是 : {text.ToString().Replace("!reply ", "")}");
                }

                if (text.Length > 6 && text.Substring(0, 6).Equals("!chat ")) {

                    string Prompt = text.ToString().Replace("!chat ", "");
                    string Request = await ChatGPT.Chat.ResponseMessageAsync(Prompt);
                    textMessage = new TextMessage(Request);
                }

                break;
        }

        if (!string.IsNullOrEmpty(textMessage.text) && textMessage.quickReply.items != null) {
            bots.PushMessage(textMessage);
        }
        else {
            bots.ReplyMessage(textMessage.text);
        }

    }//ReplyBotsMessage()





}//class

