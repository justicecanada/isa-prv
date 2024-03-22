using Newtonsoft.Json;
using System;

namespace Interview.UI
{

    public class Program
    {
        public static async Task Main(string[] args)
        {

            IHost host = null;

            try
            {

                host = CreateHostBuilder(args).Build();

                await SeedMockedData(host);
                host.Run();

            }
            catch (Exception ex)
            {
                var msgObj = GetExceptionDetails(ex);
                var msg = JsonConvert.SerializeObject(msgObj);
                ILogger logger = host.Services.GetService<ILogger<Program>>();
                logger.LogCritical(ex, msg);
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
                var seeder = scope.ServiceProvider.GetService<Interview.UI.Services.Seeder.EquitySeeder>();
                await seeder.EnsureEquities();
            }

        }

        private static object GetExceptionDetails(Exception exception)
        {

            object result = new
            {
                message = exception.Message,
                stacktrace = exception.StackTrace,
                innerException = exception.InnerException == null ? null : GetExceptionDetails(exception.InnerException)
            };

            return result;

        }

    }

}
