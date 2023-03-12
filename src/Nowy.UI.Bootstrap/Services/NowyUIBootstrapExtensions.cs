﻿using Microsoft.Extensions.DependencyInjection;
using Nowy.UI.Common.Services;

namespace Nowy.UI.Bootstrap.Services;

public static class NowyUIBootstrapExtensions
{
    public static void AddNowyUIBootstrap(this IServiceCollection services)
    {
        services.AddSingleton<IWebAssetReferenceService, BootstrapWebAssetReferenceService>();
    }

}
