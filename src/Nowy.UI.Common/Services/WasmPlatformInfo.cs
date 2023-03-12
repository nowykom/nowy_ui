namespace Nowy.UI.Common.Services;

public class WasmPlatformInfo : IPlatformInfo
{
    private readonly BrowserService _browser;

    public WasmPlatformInfo(BrowserService browser)
    {
        this._browser = browser;
    }

    public string GetPlatformName()
    {
        return "Browser";
    }

    public long GetGithubVersion()
    {
        return -1;
    }

    public async Task OpenBrowserAsync(string url)
    {
    }

    public void BeginInvokeOnMainThread(Action action) => action?.Invoke();

    public Task InvokeOnMainThreadAsync(Action action)
    {
        action?.Invoke();
        return Task.CompletedTask;
    }

    public Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
    {
        T ret = func.Invoke();
        return Task.FromResult<T>(ret);
    }

    public Task InvokeOnMainThreadAsync(Func<Task> func_task)
    {
        Task ret = func_task.Invoke();
        return ret;
    }

    public Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> func_task)
    {
        Task<T> ret = func_task.Invoke();
        return ret;
    }
}
