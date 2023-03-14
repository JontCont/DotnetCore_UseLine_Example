using Azure.Identity;

namespace StartFMS_BackendAPI.Line.WebAPI.Extensions
{
    public static class Config
    {
        /// <summary>
        /// 取得 ConnectionStrings 底下資料
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            IConfiguration config = GetConfiguration();
            return config.GetConnectionString(name);
        }

        /// <summary>
        /// 取用Config設定檔資料
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfiguration() {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            // 取得 Azure App Configuration 的連線字串
            var connectionString = builder.Build()["ConnectionStrings:AppConfig"];

            if(connectionString != null) {
                // 加入 Azure App Configuration 資料源
                builder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString)
                        // 如果您想要只加載指定的鍵，可以使用 Select 方法，例如：
                        //.Select(KeyFilter.AnyOf("MyApp:*"))
                        .ConfigureKeyVault(kv => {
                            kv.SetCredential(new DefaultAzureCredential());
                        });
                });
            }
            return builder.Build();
        }

    }
}
