using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Data
{

    public class SqlContext : DbContext
    {

        private readonly string _connectionString;

        public SqlContext(DbContextOptions<SqlContext> options, IWebHostEnvironment hostEnvironment) : base(options)
        {

            string environmentName = hostEnvironment.EnvironmentName;
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json")
                .Build();

            _connectionString = config.GetConnectionString("SQLConnectionString");

            Database.Migrate();

        }

        public SqlContext(DbContextOptions<SqlContext> options, string connectionString) : base(options)
        {

            _connectionString = connectionString;

        }

        public DbSet<Interview.Entities.Contest> Contests { get; set; }
        public DbSet<Interview.Entities.EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Interview.Entities.EmailType> EmailTypes { get; set; }
        public DbSet<Interview.Entities.Equity> Equities { get; set; }
        public DbSet<Interview.Entities.Interview> Interviews { get; set; }
        public DbSet<Interview.Entities.InterviewUser> InterviewUsers { get; set; }     
        public DbSet<Interview.Entities.Role> Roles { get; set; }
        public DbSet<Interview.Entities.Schedule> Schedules { get; set; }
        public DbSet<Interview.Entities.ScheduleType> ScheduleTypes { get; set; }
        public DbSet<Interview.Entities.UserLanguage> UserLanguages { get; set; }
        public DbSet<Interview.Entities.UserSetting> UserSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }

}
