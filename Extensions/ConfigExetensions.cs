namespace start5M.Line.WebAPI.Extensions
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
        /// 取用  appsettings.json 資料
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfiguration()
        {
            var assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .Single(o => o.EntryPoint != null);

            IConfiguration config = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddUserSecrets(assembly, optional: false)
                          .Build();
            Console.WriteLine(config["Test"]);

            return config;
        }

    }
}
