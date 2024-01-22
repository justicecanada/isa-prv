using ContainerAppsAuth.Auth.ContainerApp;
using ContainerAppsAuth.Auth.Localhost;
using ContainerAppsAuth.Services;

var builder = WebApplication.CreateBuilder(args);

// User Secrets: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&source=recommendations&tabs=windows

builder.Services.AddHttpClient();
// For configuring with certificate:
// https://www.answeroverflow.com/m/1068187115399172176


builder.Services.AddTransient<TokenManager>();
builder.Services.AddTransient<GraphManager>();
// Add services to the container.
builder.Services.AddControllersWithViews();

if (builder.Environment.EnvironmentName.ToLower() == "development")
{
    builder.Services.AddAuthentication(LocalhostAuthenticationBuilderExtensions.LOCALHOSTAUTHSCHEMENAME)
        .AddLocalhostAuth();
    builder.Services.AddAuthorization();
}
else
{
    builder.Services.AddAuthentication(EasyAuthAuthenticationBuilderExtensions.EASYAUTHSCHEMENAME)
        .AddAzureContainerAppsEasyAuth();
        builder.Services.AddAuthorization();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();
