using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using Nowy.Standard;
using Nowy.UI.Common.Services;

namespace Nowy.UI.ClientWasm.Services;

public static class NowyUIClientWasmExtensions
{
    public static void AddNowyUIClientWasm(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddNowyStandard();

        builder.Services.AddMudServices();

        builder.Services.AddHxServices();
        builder.Services.AddHxMessageBoxHost();
        builder.Services.AddHxMessenger();

        builder.Services.AddHttpClient("", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
        builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(""));
        builder.Services.AddSingleton<IPlatformInfo>(sp => new WasmPlatformInfo(sp.GetRequiredService<BrowserService>()));
        builder.Services.AddSingleton<FileService>(sp => new WasmFileService(sp.GetRequiredService<IJSRuntime>(), sp.GetRequiredService<ILogger<WasmFileService>>()));
        builder.Services.AddSingleton<BrowserService>(sp => new BrowserService(sp.GetRequiredService<IJSRuntime>()));
        builder.Services.AddSingleton<ResponsiveStateService>(sp => new ResponsiveStateService(sp.GetRequiredService<IJSRuntime>(), sp.GetRequiredService<BrowserService>()));
    }

    public static void UseNowyUIClientWasm(this WebAssemblyHost host, string? app_name = null, string? app_title = null, string? app_version = null)
    {
        StandardApp.Services = host.Services;

        StandardApp.AppName = app_name ?? StandardApp.AppName;
        StandardApp.AppVersion = app_version ?? StandardApp.AppVersion;
        StandardApp.AppTitle = app_title ?? StandardApp.AppTitle;
    }
}
