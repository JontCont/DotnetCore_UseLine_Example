using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StartFMS.Models.Backend;
using StartFMS.Models.Identity;
using StartFMS_BackendAPI.Extensions;
using System.Net.Mail;
using System.Security.Cryptography;

namespace StartFMS_BackendAPI.Controllers;

public class JsonResult {
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "";
    public string? Error { get; set; }
    public string? Token { get; set; }
}

[ApiController]
[Route("[controller]")]
public class IdentityController : Controller {
    private readonly ILogger<IdentityController> _logger;
    private readonly A00_BackendContext _context;

    public IdentityController(
        ILogger<IdentityController> logger,
        A00_BackendContext backendContext) {
        _logger = logger;
        _context = backendContext;
    }

    [HttpPost(Name = "")]
    public string PostFormIdentity(IdentityUsers identity) {
        var User = _context.A00AccountUsers
            .Where(item => IsValidEmail(identity.Users) ? item.Email == identity.Users : item.UserName == identity.Users)
            .Where(item => item.PasswordHash.ToUpper() == identity.Password.ToUpper()); 

        if (!User.ToList().Any()) {
            return JsonConvert.SerializeObject(new JsonResult {
                Success = false,
                Error = "403 Forbidden",
                Message = "驗證身分失敗。"
            });
        }

        string resultToken = new JwtHelpers().GenerateToken(identity);

        return string.IsNullOrEmpty(resultToken)
            ? JsonConvert.SerializeObject(new JsonResult {
                Success = false,
                Error = "403 Forbidden",
            })
            : JsonConvert.SerializeObject(new JsonResult {
                Success = true,
                Message = "",
                Token = resultToken
            });
    }//PostFormIdentity()

    [HttpGet(Name = "")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public string GetFormIdentity() {
        UsersAuthorize users = new UsersAuthorize(Request);
        if (users == null)
            return JsonConvert.SerializeObject(new JsonResult {
                Success = false,
                Error = "403 Forbidden",
                Message = "驗證身分失敗。"
            });
        return (users != null)
            ? JsonConvert.SerializeObject(new JsonResult { Success = true }, Formatting.None)
            : JsonConvert.SerializeObject(new JsonResult { Success = false }, Formatting.None);
    }//GetFormIdentity()



    static bool IsValidEmail(string email) {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch {
            return false;
        }
    }
}
