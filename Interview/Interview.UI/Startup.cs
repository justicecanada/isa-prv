using AutoMapper;
using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Filters;
using Interview.UI.Models;
using Interview.UI.Services.Automapper;
using Interview.UI.Services.DAL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data.SqlTypes;

namespace Interview.UI
{
    
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<DalSql>();
            services.AddDbContext<SqlContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString")));

            services.AddAutoMapper(typeof(MapperConfig));

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {

            app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Contests}/{action=Index}");
            });

        }

    }

}
