using System.Reflection;
using Nowy.UI.Common.Services;

namespace Nowy.UI.Bootstrap.Services;

public sealed class BootstrapWebAssetReferenceService : IWebAssetReferenceService
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
            $"_content/Nowy.UI.Bootstrap/output/init.css?start_time={this.GetStartTime()}",
            $"_content/Nowy.UI.Bootstrap/output/bundle-{this.JavascriptTheme.ToString().ToLower().Replace("_", "-")}-{this.JavascriptFramework.ToString().ToLower().Replace("_", "-")}.css?start_time={this.GetStartTime()}",
            $"css/app.css?start_time={this.GetStartTime()}",
            $"{this.WebAssemblyEntryAssembly?.GetName().Name}.styles.css?start_time={this.GetStartTime()}",
        };

        return ret;
    }

    public IReadOnlyList<string> GetJavascriptPaths()
    {
        List<string> ret = new()
        {
            $"_content/Nowy.UI.Bootstrap/output/init.js?start_time={this.GetStartTime()}",
            $"_content/Nowy.UI.Bootstrap/output/bundle-{this.JavascriptTheme.ToString().ToLower().Replace("_", "-")}-{this.JavascriptFramework.ToString().ToLower().Replace("_", "-")}.js?start_time={this.GetStartTime()}",
            $"js/app.js?start_time={this.GetStartTime()}",
            $"{this.WebAssemblyEntryAssembly?.GetName().Name}.styles.css?start_time={this.GetStartTime()}",
        };

        return ret;
    }
}

public enum BootstrapJavascriptTheme
{
    NONE = 0,
    LR,
    TS,
    NOWY,
}

public enum BootstrapJavascriptFramework
{
    NONE = 0,
    BOOTSTRAP5_FLUENTUI,
    BOOTSTRAP5,
}
