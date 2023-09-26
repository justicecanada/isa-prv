using Microsoft.AspNetCore.Hosting;

namespace Interview.UI
{

    public class Program
    {
        public static async Task Main(string[] args)
        {

            try
            {
                
                IHost host = CreateHostBuilder(args).Build();

                await SeedRoles(host);
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

        #region Mocked up data

        private static async Task SeedRoles(IHost host)
        {

            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<Interview.UI.Services.Mock.RoleSeeder>();
                await seeder.EnsureRoles();
            }

        }

        #endregion

    }

}
