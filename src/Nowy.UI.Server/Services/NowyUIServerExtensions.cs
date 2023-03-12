using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nowy.UI.Server.Services;

public static class NowyUIServerExtensions
{
    public static void AddNowyUIServer(this WebApplicationBuilder builder)
    {
        // builder.Services.AddSingleton<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress), });

        builder.Services.AddHttpClient("", client => { });
        builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(""));

        builder.Services.AddSingleton<StartupTimeService>();

        builder.Services.AddControllersWithViews()
            .AddApplicationPart(typeof(NowyUIServerExtensions).Assembly)
            .AddRazorRuntimeCompilation()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = null;
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });


        builder.Services.AddRazorPages();

        // .AddNewtonsoftJson(json_options => ConfigHelper.CreateJsonSerializerSettings(json_options.SerializerSettings));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
    }

    public static void UseNowyUIServer(this WebApplication app, bool host_blazor_wasm = false,
        string? app_name = null, string? app_title = null, string? app_version = null)
    {
        if (host_blazor_wasm)
        {
            app.UseBlazorFrameworkFiles();
        }

        app.UseStaticFiles();

        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        if (host_blazor_wasm)
        {
            // app.MapFallbackToFile("index.html");
            app.MapFallbackToController("Index", "Home");
        }

        StandardApp.Services = app.Services;

        StandardApp.AppName = app_name ?? StandardApp.AppName;
        StandardApp.AppVersion = app_version ?? StandardApp.AppVersion;
        StandardApp.AppTitle = app_title ?? StandardApp.AppTitle;

        // Log.StorageEnabled = false;
        // Log.ShouldPrintMethod = false;
        // Log.ShouldPrintTimestamp = false;
        // Log.SetStarted();
    }
}
