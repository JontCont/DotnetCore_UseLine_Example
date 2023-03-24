using isRock.LineBot;

namespace StartFMS_BackendAPI.Line.WebAPI.Extensions.LineBots;
public class LineBots {
    // Public 設定檔
    public string ChannelToken { get; set; }
    public string AdminUserID { get; set; }
    public string ReplyUserID { get; set; }
    public Stream STREAM { get; set; }
    public ReceivedMessage ReceivedMessage { get; set; }

    // Private 設定檔
    private Bot LINE_BOT { get; set; }
    private string ADMIN_TOKEN_ID { get; set; }

    public LineBots Load() {
        LINE_BOT = new Bot(ChannelToken);
        ADMIN_TOKEN_ID = AdminUserID;
        return this;
    }

    public async Task<LineBots> LoadAsync(Stream stream) {
        LINE_BOT = new Bot(ChannelToken);
        ADMIN_TOKEN_ID = AdminUserID;
        STREAM = stream;

        // 確認 Post 內容
        try {
            //取得 http Post 
            using (StreamReader reader = new(STREAM, System.Text.Encoding.UTF8)) {
                string strBody = await reader.ReadToEndAsync();
                if (reader == null || string.IsNullOrEmpty(strBody))
                    throw new ArgumentNullException("Mandatory parameter", nameof(strBody)); ;
                ReceivedMessage = Utility.Parsing(strBody);
            }
        }
        catch (Exception ex) {
            PushMessage(ex.Message);
        }
        return this;
    }

    public void PushMessage(string message) {
        LINE_BOT.PushMessage(ADMIN_TOKEN_ID, message);
    }

    public void PushMessage(TextMessage message) {
        LINE_BOT.PushMessage(ADMIN_TOKEN_ID, message);
    }

    public void PushMessage(string TokenId, string message) {
        LINE_BOT.PushMessage(TokenId, message);
    }


    public void ReplyMessage(string message) {
        LINE_BOT.ReplyMessage(ReplyUserID, message);
    }

    public void ReplyMessage(string TokenId, string message) {
        LINE_BOT.ReplyMessage(TokenId, message);
    }
}//class
