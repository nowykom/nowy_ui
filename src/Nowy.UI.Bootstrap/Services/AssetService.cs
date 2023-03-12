using System.Reflection;

namespace Nowy.UI.Bootstrap.Services;

public sealed class AssetService
{
    private static long? _start_time;
    public long GetStartTime() => _start_time ??= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public JavascriptFramework JavascriptFramework { get; set; }
    public JavascriptTheme JavascriptTheme { get; set; }
    public Assembly? WebAssemblyEntryAssembly { get; set; }
}

public enum JavascriptTheme
{
    NONE = 0,
    LR,
    TS,
    NOWY,
}

public enum JavascriptFramework
{
    NONE = 0,
    BOOTSTRAP5_FLUENTUI,
    BOOTSTRAP5,
}
