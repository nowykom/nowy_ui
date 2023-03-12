namespace Nowy.UI.Common.Services;

public interface IPlatformInfo
{
    string GetPlatformName();
    long GetGithubVersion();
    Task OpenBrowserAsync(string url);

    void BeginInvokeOnMainThread(Action action);
    Task InvokeOnMainThreadAsync(Action action);
    Task<T> InvokeOnMainThreadAsync<T>(Func<T> func);
    Task InvokeOnMainThreadAsync(Func<Task> func_task);
    Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> func_task);
}
