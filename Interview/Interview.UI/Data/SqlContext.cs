using Interview.UI.Services.Mock.Identity;
using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Data
{

    public class SqlContext : DbContext
    {

        private readonly string _connectionString;

        public SqlContext(DbContextOptions<SqlContext> options, IConfiguration config) : base(options)
        {

            _connectionString = config["sql-connection-string"];

            //Database.EnsureDeletedAsync().Wait();
            //Database.EnsureCreatedAsync().Wait();
            //Database.MigrateAsync().Wait();

        }

        public SqlContext(DbContextOptions<SqlContext> options, string connectionString) : base(options)
        {

            _connectionString = connectionString;

        }

        public DbSet<Interview.Entities.Process> Processes { get; set; }
        public DbSet<Interview.Entities.ProcessGroup> ProcessGroups { get; set; }
        public DbSet<Interview.Entities.EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Interview.Entities.Equity> Equities { get; set; } 
        public DbSet<Interview.Entities.Group> Groups { get; set; }
        public DbSet<Interview.Entities.GroupOwner> GroupsOwners { get; set; }
        public DbSet<Interview.Entities.Interview> Interviews { get; set; }
        public DbSet<Interview.Entities.InterviewUser> InterviewUsers { get; set; }     
        public DbSet<Interview.Entities.Schedule> Schedules { get; set; }
        public DbSet<Interview.Entities.RoleUser> RoleUsers { get; set; }
        public DbSet<Interview.Entities.RoleUserEquity> RoleUserEquities { get; set; }

        // Mocked up data
        public DbSet<Interview.UI.Services.Mock.Identity.MockUser> MockUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }

}
