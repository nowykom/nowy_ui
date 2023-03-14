using System.Reflection;
using Nowy.UI.Common.Services;

namespace Nowy.UI.Maps.Services;

public sealed class MapsWebAssetReferenceService : IWebAssetReferenceService
{
    public BootstrapJavascriptFramework JavascriptFramework { get; set; }
    public BootstrapJavascriptTheme JavascriptTheme { get; set; }
    public Assembly? WebAssemblyEntryAssembly { get; set; }

    public string? InitialPageTitle { get; set; }

    private static long? _start_time;
    public long GetStartTime() => _start_time ??= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public IReadOnlyList<string> GetCssPaths()
    {
        List<string> ret = new()
        {
            $"_content/Nowy.UI.Maps/output/module-leaflet.css?start_time={this.GetStartTime()}",
        };

        return ret;
    }

    public IReadOnlyList<string> GetJavascriptPaths()
    {
        List<string> ret = new()
        {
            $"_content/Nowy.UI.Maps/output/module-leaflet.js?start_time={this.GetStartTime()}",
        };

        return ret;
    }
}
