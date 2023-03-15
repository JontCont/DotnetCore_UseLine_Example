using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StartFMS.Models.Identity;
using StartFMS_BackendAPI.Extensions;
namespace StartFMS_BackendAPI.Extensions;
public class JwtHelpers
{
    private readonly IConfiguration Configuration;

    public JwtHelpers()
    {
        this.Configuration = Config.GetConfiguration();
    }

    /// <summary>
    /// 單一使用者名稱登入。
    /// </summary>
    /// <param name="userName">使用名稱</param>
    /// <param name="expireMinutes">時效</param>
    /// <returns></returns>
    public string GenerateToken(string userName, int expireMinutes = 30)
    {
        var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
        var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");

        // Configuring "Claims" to your JWT Token
        var claims = new List<Claim>();

        // In RFC 7519 (Section#4), there are defined 7 built-in Claims, but we mostly use 2 of them.
        //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName)); // User.Identity.Name
        //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
        //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
        //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
        //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

        // The "NameId" claim is usually unnecessary.
        //claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userName));

        // This Claim can be replaced by JwtRegisteredClaimNames.Sub, so it's redundant.
        //claims.Add(new Claim(ClaimTypes.Name, userName));

        // TODO: You can define your "roles" to your Claims.
        claims.Add(new Claim("roles", "Users"));

        var userClaimsIdentity = new ClaimsIdentity(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Create SecurityTokenDescriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            //Audience = issuer, // Sometimes you don't have to define Audience.
            //NotBefore = DateTime.Now, // Default is DateTime.Now
            //IssuedAt = DateTime.Now, // Default is DateTime.Now
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = signingCredentials,
        };

        // Generate a JWT securityToken, than get the serialized Token result (string)
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var serializeToken = tokenHandler.WriteToken(securityToken);

        return serializeToken;
    }

    /// <summary>
    /// 從 BDP080 取得資料登入 
    /// </summary>
    /// <param name="userAutos">使用者驗證</param>
    /// <param name="expireMinutes">時效</param>
    /// <returns></returns>
    public string GenerateToken(IdentityUsers userAutos, int expireMinutes = 30)
    {
        var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
        var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");

        // Configuring "Claims" to your JWT Token
        var claims = new List<Claim>() {
            // In RFC 7519 (Section#4), there are defined 7 built-in Claims, but we mostly use 2 of them.
            //new Claim(JwtRegisteredClaimNames.Iss, issuer),
            new Claim(JwtRegisteredClaimNames.Sub, userAutos.Users),// User.Identity.Name
            //new Claim(JwtRegisteredClaimNames.Aud, "The Audience"),
            //new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()),
            //new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // 必須為數字
            //new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // 必須為數字

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),// JWT ID

            // The "NameId" claim is usually unnecessary.
            // new Claim(JwtRegisteredClaimNames.NameId, userName),

            // This Claim can be replaced by JwtRegisteredClaimNames.Sub, so it's redundant.
            // new Claim(ClaimTypes.Name, userName),

            // TODO: You can define your "roles" to your Claims.
            //new Claim("roles",  userAutos.User),
            //new Claim("token",  userAutos.Token),
        };

        var userClaimsIdentity = new ClaimsIdentity(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Create SecurityTokenDescriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            // Audience = issuer, // Sometimes you don't have to define Audience.
            // NotBefore = DateTime.Now, // Default is DateTime.Now
            // IssuedAt = DateTime.Now, // Default is DateTime.Now
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = signingCredentials
        };

        // Generate a JWT securityToken, than get the serialized Token result (string)
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var serializeToken = tokenHandler.WriteToken(securityToken);

        return serializeToken;
    }

    //------------------------

    /// <summary>
    /// 數據傳輸轉出的資料
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static JwtTokenModel GetPayLoadData(SecurityToken token)
    {
        JwtSecurityToken jwt_token = token as JwtSecurityToken;
        if (jwt_token != null)
        {
            return GetPayLoadData(jwt_token);
        }
        return new JwtTokenModel();
    }

    /// <summary>
    /// 數據傳輸轉出的資料
    /// </summary>
    /// <param name="jwt_token"></param>
    /// <returns></returns>
    public static JwtTokenModel GetPayLoadData(JwtSecurityToken jwt_token)
    {
        var payload = jwt_token.Payload;
        string payload_json = JsonConvert.SerializeObject(payload);

        var model = JsonConvert.DeserializeObject<JwtTokenModel>(payload_json);

        return model ?? new JwtTokenModel();

    }


}