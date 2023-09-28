namespace Interview.UI
{

    public class Program
    {
        public static async Task Main(string[] args)
        {

            try
            {

                IHost host = CreateHostBuilder(args).Build();

                await SeedMockedData(host);
                await SeedMockUsers(host);
                host.Run();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task SeedMockedData(IHost host)
        {

            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<Interview.UI.Services.Mock.MockSeeder>();
                await seeder.EnsureRoles();
                await seeder.EnsureUserLanguages();
                //await seeder.EnsureEquities();
            }

        }

        private static async Task SeedMockUsers(IHost host)
        {

            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<Interview.UI.Services.Mock.Identity.MockIdentitySeeder>();
                await seeder.EnsureUsers();
            }

        }

    }

}
