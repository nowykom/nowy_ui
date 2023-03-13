using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Nowy.UI.Common.Services;

namespace Nowy.UI.Maps.Services;

public static class NowyUIMapsExtensions
{
    public static void AddNowyUIMaps(this IServiceCollection services)
    {
        services.AddSingleton<MapsWebAssetReferenceService>();
        services.AddSingleton<IWebAssetReferenceService>(sp => sp.GetRequiredService<MapsWebAssetReferenceService>());

        services.AddSingleton<GeocodingService>(sp => new GeocodingService(sp.GetRequiredService<IHttpClientFactory>().CreateClient(), sp.GetRequiredService<ISnackbar>()));
    }
}
