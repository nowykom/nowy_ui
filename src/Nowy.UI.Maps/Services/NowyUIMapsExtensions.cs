using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Nowy.UI.Maps.Services;

public static class NowyUIMapsExtensions
{
    public static void AddNowyUIMaps(this IServiceCollection services)
    {
        services.AddSingleton<GeocodingService>(sp => new GeocodingService(sp.GetRequiredService<IHttpClientFactory>().CreateClient(), sp.GetRequiredService<ISnackbar>()));
    }
}
