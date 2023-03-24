using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenAI.GPT3.Extensions;
using StartFMS.Models;
using StartFMS_BackendAPI.Extensions;
using StartFMS_BackendAPI.Line.WebAPI.Extensions.LineBots;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var config = Config.GetConfiguration(); //加入設定檔
builder.Configuration.AddUserSecrets<Program>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenAIService();

//add core content
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        builder => {
            builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
        });

    options.AddPolicy("AnotherPolicy",
        builder => {
            builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
        options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

        options.TokenValidationParameters = new TokenValidationParameters {
            // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

            // 一般我們都會驗證 Issuer
            ValidateIssuer = true,
            ValidIssuer = config.GetValue<string>("JwtSettings:Issuer"),

            // 通常不太需要驗證 Audience
            ValidateAudience = false,
            //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

            // 一般我們都會驗證 Token 的有效期間
            ValidateLifetime = true,

            // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
            ValidateIssuerSigningKey = false,

            // "1234567890123456" 應該從 IConfiguration 取得
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetConfiguration().GetValue<string>("JwtSettings:SignKey")))
        };
        options.Events = new JwtBearerEvents {
            OnMessageReceived = context => {
                var accessToken = context.Request.Cookies["x-access-token"];

                if (!string.IsNullOrEmpty(accessToken)) {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(options => {
        options.EventsType = typeof(CookieAuthenticationEventsExetensions);
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        options.Cookie.Name = "user-session";
        options.SlidingExpiration = true;
    });

//設定參數
var backend = new BackendContext() {
    ConnectionString = config.GetValue<string>("ConnectionStrings:Default")
};
builder.Services.AddSingleton<BackendContext>(backend);

var lineBots = new LineBots() {
    ChannelToken = config.GetValue<string>("Line:Bots:channelToken"),
    AdminUserID = config.GetValue<string>("Line:Bots:adminUserID")
};
builder.Services.AddSingleton<LineBots>(lineBots);


builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Start Five Minutes Backend API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
