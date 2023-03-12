namespace Nowy.UI.Server.Services;

public sealed class StartupTimeService
{
    private static long? _start_time;
    public long GetStartTime() => _start_time ??= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
