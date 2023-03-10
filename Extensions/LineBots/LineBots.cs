using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace start5M.Line.WebAPI.Extensions.LineBots
{
    public class LineBots
    {
        public Bot LINE_BOT { get; set; }
        public string ADMIN_TOKEN_ID { get; set; }

        public Stream STREAM{ get; set; }
        public LineBots Load()
        {
            string channelToken = Config.GetConfiguration().GetValue<string>("Line:Bots:channelToken");
            string adminToken = Config.GetConfiguration().GetValue<string>("Line:Bots:adminUserID");
            LINE_BOT = new Bot(channelToken);
            ADMIN_TOKEN_ID = adminToken;
            return this;
        }

        public LineBots Load(Stream stream)
        {
            string channelToken = Config.GetConfiguration().GetValue<string>("Line:Bots:channelToken");
            string adminToken = Config.GetConfiguration().GetValue<string>("Line:Bots:adminUserID");
            LINE_BOT = new Bot(channelToken);
            ADMIN_TOKEN_ID = adminToken;
            STREAM = stream;
            return this;
        }

    }
}
