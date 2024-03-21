using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Interview.UI.Data
{

    public class SqlContext : DbContext
    {

        private readonly string _connectionString;

        public SqlContext(DbContextOptions<SqlContext> options, IConfiguration config, IWebHostEnvironment hostEnvironment) : base(options)
        {
            // For regular dependency injection
            _connectionString = config["sql-connection-string"];
        }

        public SqlContext(DbContextOptions<SqlContext> options, string connectionString) : base(options)
        {
            // For Interview.UI.Tests
            _connectionString = connectionString;
        }

        public DbSet<Interview.Entities.Process> Processes { get; set; }
        public DbSet<Interview.Entities.ProcessGroup> ProcessGroups { get; set; }
        public DbSet<Interview.Entities.EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Interview.Entities.Equity> Equities { get; set; }
        public DbSet<Interview.Entities.ExternalUser> ExternalUsers { get; set; }
        public DbSet<Interview.Entities.Group> Groups { get; set; }
        public DbSet<Interview.Entities.GroupOwner> GroupsOwners { get; set; }
        public DbSet<Interview.Entities.Interview> Interviews { get; set; }
        public DbSet<Interview.Entities.InterviewUser> InterviewUsers { get; set; }     
        public DbSet<Interview.Entities.InterviewUserEmail> InterviewUserEmails { get; set; }
        public DbSet<Interview.Entities.Schedule> Schedules { get; set; }
        public DbSet<Interview.Entities.RoleUser> RoleUsers { get; set; }
        public DbSet<Interview.Entities.RoleUserEquity> RoleUserEquities { get; set; }
        public DbSet<Interview.Entities.InternalUser> InternalUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }

}
