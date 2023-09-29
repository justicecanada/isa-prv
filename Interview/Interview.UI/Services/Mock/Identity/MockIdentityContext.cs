using Interview.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Services.Mock.Identity
{
    
    public class MockIdentityContext : DbContext
    {

        private readonly string _connectionString;

        public MockIdentityContext(DbContextOptions<MockIdentityContext> options, IWebHostEnvironment hostEnvironment) : base(options)
        {

            string environmentName = hostEnvironment.EnvironmentName;
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json")
                .Build();

            _connectionString = config.GetConnectionString("SQLConnectionString");

            //Database.Migrate();

        }

        public MockIdentityContext(DbContextOptions<MockIdentityContext> options, string connectionString) : base(options)
        {

            _connectionString = connectionString;

        }

        public DbSet<MockUser> MockUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }

}
