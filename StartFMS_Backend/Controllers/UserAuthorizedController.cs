using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StartFMS.Models.Backend;
using StartFMS_BackendAPI.Extensions;
using System.Text.Json.Serialization;

namespace StartFMS_BackendAPI.Controllers;

/// <summary>
/// 驗證需回傳內容。
/// </summary>
public class userAuthentication {
    //下方回傳資料需用加密處理才能轉換 JWT Token
    [JsonPropertyName("user")]
    public string User { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("role")]
    public string Roles { get; set; }

}

// 需轉成 Token 資料， 須配合 JwtHelpers
public class userAuthenticationJwt {
    public string User { get; set; }
    public string Token { get; set; }
    public string Roles { get; set; }
}

[ApiController]
[Route("[controller]/[action]")]
public class UserAuthorizedController : Controller {

    private readonly ILogger<UserAuthorizedController> _logger;
    private readonly A00_BackendContext _context;

    public UserAuthorizedController(
        ILogger<UserAuthorizedController> logger,
        A00_BackendContext A00_BackendContext) {
        _logger = logger;
        _context = A00_BackendContext;
    }

    [HttpGet]
    public string Example(string key) {
        JwtHelpers jwt = new JwtHelpers();
        string token = jwt.GenerateToken(key);
        return JsonConvert.SerializeObject(new { token });
    }


    [HttpPost]
    public string JWTAuthentication([FromBody] userAuthentication userAuth) {
        JwtHelpers jwt = new();
        string User = (userAuth.User);
        string Token = (userAuth.Token);
        string resultToken = "";

        //判斷NULL or Empty
        if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Token)) {
            return JsonConvert.SerializeObject(
                new {
                    success = false,
                    message = "Error : The information sent to the server is incorrect, please reconfirm."
                });
        }

        A00AccountUser? accountUser = _context.A00AccountUsers.Where(row => row.UserName == User && row.Id == Token)?.FirstOrDefault();
        if (accountUser != null && !string.IsNullOrEmpty(accountUser.UserName)) {
#pragma warning disable CS8604 // 可能有 Null 參考引數。
            resultToken = jwt.GenerateToken(accountUser.UserName);
#pragma warning restore CS8604 // 可能有 Null 參考引數。
        }

        return (string.IsNullOrEmpty(resultToken)) ?
            JsonConvert.SerializeObject(new { success = false, message = "Error : Failed to get KEY. Please reconfirm user information." }) :
            JsonConvert.SerializeObject(new { success = true, message = "", token = resultToken });
    }


    [HttpPost("/Login")]
    public string CookiesAuthentication([FromBody] userAuthentication userAuth) {
        JwtHelpers jwt = new();
        string User = (userAuth.User);
        string Token = (userAuth.Token);
        string Roles = (userAuth.Roles);
        string resultToken = "";


        if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Token)) {
            return JsonConvert.SerializeObject(new {
                success = false,
                message = "Error : The information sent to the server is incorrect, please reconfirm."
            });
        }

        A00AccountUser? accountUser = _context.A00AccountUsers.Where(row => row.UserName == User && row.Id == Token)?.FirstOrDefault();
        if (accountUser != null && string.IsNullOrEmpty(accountUser.UserName)) {
#pragma warning disable CS8604 // 可能有 Null 參考引數。
            resultToken = jwt.GenerateToken(accountUser.UserName);
#pragma warning restore CS8604 // 可能有 Null 參考引數。
        }

        Response.Cookies.Append("x-access-token", resultToken,
          new CookieOptions() {
              Path = "/",
              HttpOnly = true
          });


        return (string.IsNullOrEmpty(resultToken)) ?
            JsonConvert.SerializeObject(new { success = false, message = "Error : Failed to get KEY. Please reconfirm user information." }) :
            JsonConvert.SerializeObject(new { success = true, message = "", token = resultToken });
    }
}//class
