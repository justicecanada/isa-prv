using AutoMapper;
using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Filters;
using Interview.UI.Models;
using Interview.UI.Models.CustomValidation;
using Interview.UI.Services.Automapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Globalization;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Interview.UI.Services.Mock;
using Interview.UI.Services.Mock.Identity;

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

            IMvcBuilder builder = services.AddMvc();
            ConfigureLocalizationServices(services, builder);

            services.AddTransient<DalSql>();
            services.AddDbContext<SqlContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString")));

            services.AddAutoMapper(typeof(MapperConfig));

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            })
                .AddRazorRuntimeCompilation();

            services.AddScoped<IState, SessionState>();

            // Mocked Services
            services.AddTransient<MockSeeder>();
            services.AddDbContext<MockIdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString")));
            services.AddTransient<MockIdentitySeeder>();

            // WET
            services.AddModelAccessor();
            services.ConfigureGoCTemplateRequestLocalization(); // >= v2.3.0
        }

        private void ConfigureLocalizationServices(IServiceCollection services, IMvcBuilder builder)
        {

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-CA"),
                    new CultureInfo("fr-CA")
                };
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-CA");
                //options.SetDefaultCulture("en-CA");
            });
            CultureInfo cultureInfo = new CultureInfo("en-CA");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            builder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization(); ;

            services.Configure<LocalizationOptions>(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {

            app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Default}/{action=Index}");
            });

            // WET
            app.UseRequestLocalization(); // >= v2.3.0
            app.UseRequestLocalization(CultureConfiguration.GetLocalizationOptions()); // <= v2.3.0

        }

    }

}
