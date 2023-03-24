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
var config = Config.GetConfiguration(); //�[�J�]�w��
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
        // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
        options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

        options.TokenValidationParameters = new TokenValidationParameters {
            // �z�L�o���ŧi�A�N�i�H�q "sub" ���Ȩó]�w�� User.Identity.Name
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            // �z�L�o���ŧi�A�N�i�H�q "roles" ���ȡA�åi�� [Authorize] �P�_����
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

            // �@��ڭ̳��|���� Issuer
            ValidateIssuer = true,
            ValidIssuer = config.GetValue<string>("JwtSettings:Issuer"),

            // �q�`���ӻݭn���� Audience
            ValidateAudience = false,
            //ValidAudience = "JwtAuthDemo", // �����ҴN���ݭn��g

            // �@��ڭ̳��|���� Token �����Ĵ���
            ValidateLifetime = true,

            // �p�G Token ���]�t key �~�ݭn���ҡA�@�볣�u��ñ���Ӥw
            ValidateIssuerSigningKey = false,

            // "1234567890123456" ���ӱq IConfiguration ���o
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

//�]�w�Ѽ�
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
