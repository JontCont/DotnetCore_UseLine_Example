using isRock.LineBot;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace start5M.Line.WebAPI.Extensions.LineBots
{
    public static class LineBotsMessage
    {
        public static string Message(this LineBots bots)
        {
            List<MessageBase> repMessage = new List<MessageBase>();
            StringBuilder strBuilder = new StringBuilder();
            string strBody = "";

            try
            {
                //取得 http Post 
                using (StreamReader reader = new(bots.STREAM, System.Text.Encoding.UTF8))
                {
                    strBody = reader.ReadToEndAsync().Result;
                    if (reader == null || string.IsNullOrEmpty(strBody))
                        return JsonConvert.SerializeObject(new { success = false, message = "error : message empty " });
                }
            }
            catch (Exception ex)
            {
                bots.LINE_BOT.PushMessage(bots.ADMIN_TOKEN_ID, ex.Message);
                return JsonConvert.SerializeObject(new { success = false, message = ex.Message });
            }


            //RawData(should be JSON)
            var ReceivedMessage = Utility.Parsing(strBody);
            if (ReceivedMessage == null) return JsonConvert.SerializeObject(new { success = false, message = "error : message empty " });

            var LineEvent = ReceivedMessage.events.FirstOrDefault();
            if (LineEvent == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "error : not found event ! " });
            }
            bots.LINE_BOT.ReplyBotsMessage(LineEvent);

            return JsonConvert.SerializeObject(new { success = true, message = "" });
        }

        private static async void ReplyBotsMessage(this Bot bot, Event lineEvent)
        {
            TextMessage textMessage = new("");

            switch (lineEvent.type)
            {
                case "join":
                    textMessage = new TextMessage($"大家好啊~");
                    break;
                case "message":
                    string text = lineEvent.message.text;
                    if (text == null) break;
                    if (text.Contains("!喵說"))
                    {
                        textMessage = new TextMessage($"您說的是 : {text.ToString().Replace("!喵說", "")}");
                    }
                    if (text.Contains("!請回答")) {
                        string Prompt = text.ToString().Replace("!請回答", "");
                        string Request = await ChatGPT.Chat.ResponseMessageAsync(Prompt);
                        textMessage = new TextMessage(Request);
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(textMessage.text))
            {
                bot.ReplyMessage(lineEvent.replyToken, textMessage);
            }
            //textMessage = new($"你回覆的訊息無法判讀，請重新輸入!!");
        }


    }
}
