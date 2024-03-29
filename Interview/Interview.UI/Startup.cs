﻿using AutoMapper;
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
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Globalization;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.Options;
using Interview.UI.Services.Seeder;
using Interview.UI.Services.Graph;
using Interview.UI.Auth;
using Interview.UI.Services.Stats;

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

            var builder = services.AddMvc();
            
            ConfigureLocalizationServices(services, builder);
            builder.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            //// This code will only run during app start up. If the database doesn't exist, it will ensure it is created up to the latest migration.
            //// If the database exists, it will ensure it is migrated to the latest migration. 
            //// This is done because the app is containerized, so any deployments with EF migrations will ensure the database schema is up to date
            //// for all downstream processing.
            //SqlContext sqlContext = new SqlContext(new DbContextOptionsBuilder<SqlContext>().Options, Configuration["sql-connection-string"]);
            //sqlContext.Database.Migrate();

            services.AddTransient<DalSql>();
            services.AddDbContext<SqlContext>(options =>
                options.UseSqlServer(Configuration["sql-connection-string"]));

            builder.Services.AddAuthentication(AuthenticationBuilderExtensions.AUTHSCHEMENAME)
                    .AddAuth();
            builder.Services.AddAuthorization();

            services.AddAutoMapper(typeof(MapperConfig));

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMilliseconds(Configuration.GetValue<double>("SessionTimeoutOptions:DotNetSessionInMilliseconds"));
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                options.Filters.Add(typeof(ViewExceptionFilter));
                options.Filters.Add(typeof(AsyncExceptionFilter));
                options.Filters.Add(typeof(LanguageFilter));
            })
                .AddRazorRuntimeCompilation();

            services.AddHttpClient();
            services.AddScoped<IState, SessionState>();
            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            services.Configure<EmailManagerOptions>(Configuration.GetSection("EmailManagerOptions"));
            services.Configure<SessionTimeoutOptions>(Configuration.GetSection("SessionTimeoutOptions"));
            services.AddScoped<IToken, TokenManager>();
            services.AddScoped<IUsers, UserManager>();
            services.AddScoped<IEmails, EmailManager>();
            services.AddScoped<IStats, StatsManager>();
            services.AddTransient<IOptions, JsonOptions>();
            services.AddTransient<EquitySeeder>();

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

            app.UseRequestLocalization(); // >= v2.3.0
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}");
            });

            // Run EF Migrations
            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var context = services.GetRequiredService<SqlContext>();
            //    context.Database.EnsureCreated();
            //    context.Database.Migrate();
            //}

        }

    }

}
