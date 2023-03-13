using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Nowy.UI.Common.Services;

public class BrowserService
{
    public event EventHandler<BrowserWindowResizedEventArgs>? WindowResized;
    public event EventHandler<BrowserIntersectionChangedEventArgs>? IntersectionChanged;

    private readonly IJSInProcessRuntime _js;
    private IJSInProcessObjectReference? _module;

    private bool _window_resize_is_initialized;

    public BrowserService(IJSRuntime js)
    {
        this._js = ( js as IJSInProcessRuntime ) ?? throw new ArgumentNullException(nameof(js));

        this.InitAsync().Forget();
    }

    public async Task InitAsync()
    {
        this._module ??= await this._js.InvokeAsync<IJSInProcessObjectReference>("import", "./_content/Nowy.UI.Common/output/Services/BrowserService.js");
    }

    public async Task<BrowserWindowSize> GetWindowSize()
    {
        if (this._js is { } js)
        {
            return await js.InvokeAsync<BrowserWindowSize>("get_window_size");
        }
        else
        {
            return new BrowserWindowSize();
        }
    }

    public async Task<(double Lat, double Lng)?> GetLocationAsync(TimeSpan timeout)
    {
        TaskCompletionSource<string> tcs = new();
        DotNetObjectReference<JavascriptPromiseHandler> promise_handler = DotNetObjectReference.Create(new JavascriptPromiseHandler(tcs));
        await this._js.InvokeAsync<object>("get_location_async", promise_handler);

        string ret = await tcs.Task.TimeoutAfter(timeout);

        bool _parse(string s, out double parsed_lat, out double parsed_lng)
        {
            if (!string.IsNullOrEmpty(s) && s.All(o => char.IsDigit(o) || o == ',' || o == '.') && s.Split(',') is { Length: 2 } o2)
            {
                parsed_lat = o2[0].Trim().ToDouble();
                parsed_lng = o2[1].Trim().ToDouble();
                return true;
            }

            parsed_lat = 0;
            parsed_lng = 0;
            return false;
        }

        if (_parse(ret, out double parsed_lat, out double parsed_lng))
        {
            return ( Lat: parsed_lat, Lng: parsed_lng );
        }

        return null;
    }

    public async Task SendWindowResizeAsync()
    {
        await Task.CompletedTask;

        await this._js.InvokeVoidAsync("send_window_resize");
    }

    public async Task InitWindowResizedEventAsync()
    {
        if (!this._window_resize_is_initialized)
        {
            this._window_resize_is_initialized = true;

            DotNetObjectReference<JavascriptWindowResizedEventHandler> event_handler = DotNetObjectReference.Create(new JavascriptWindowResizedEventHandler(this));
            await this._js.InvokeAsync<object>("init_window_resize_event", event_handler);
        }
    }

    public async Task AddIntersectionObserver(ElementReference element)
    {
        DotNetObjectReference<JavascriptIntersectionObserverEventHandler> event_handler =
            DotNetObjectReference.Create(new JavascriptIntersectionObserverEventHandler(this, element));
        await this._js.InvokeAsync<object>("add_intersection_observer", event_handler, element);
    }

    public async Task RemoveIntersectionObserver(ElementReference element)
    {
        await this._js.InvokeAsync<object>("remove_intersection_observer", element);
    }

    public void SendWindowResized(double w, double h)
    {
        this.WindowResized?.Invoke(this, new BrowserWindowResizedEventArgs(new BrowserWindowSize { Width = (int)w, Height = (int)h }));
    }

    public void SendIntersectionChanged(ElementReference element, bool is_intersecting, double intersection_ratio)
    {
        this.IntersectionChanged?.Invoke(this, new BrowserIntersectionChangedEventArgs(element, is_intersecting, intersection_ratio));
    }
}

public class JavascriptPromiseHandler
{
    private readonly TaskCompletionSource<string> _tcs;

    public JavascriptPromiseHandler(TaskCompletionSource<string> tcs)
    {
        this._tcs = tcs;
    }

    [JSInvokable(nameof(SetResult))]
    public void SetResult(string json)
    {
        this._tcs.SetResult(json);
    }
}

public class JavascriptWindowResizedEventHandler
{
    private readonly BrowserService _browser_service;

    public JavascriptWindowResizedEventHandler(BrowserService browser_service)
    {
        _browser_service = browser_service;
    }

    [JSInvokable(nameof(SetSize))]
    public void SetSize(double w, double h)
    {
        _browser_service.SendWindowResized(w, h);
    }
}

public class JavascriptIntersectionObserverEventHandler
{
    private readonly BrowserService _browser_service;
    private readonly ElementReference _element;

    public JavascriptIntersectionObserverEventHandler(BrowserService browser_service, ElementReference element)
    {
        _browser_service = browser_service;
        _element = element;
    }

    [JSInvokable(nameof(SendIntersectionChanged))]
    public void SendIntersectionChanged(bool is_intersecting, double intersection_ratio)
    {
        _browser_service.SendIntersectionChanged(this._element, is_intersecting, intersection_ratio);
    }
}

public class BrowserWindowSize
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class BrowserWindowResizedEventArgs : EventArgs
{
    public readonly BrowserWindowSize Size;

    public BrowserWindowResizedEventArgs(BrowserWindowSize size)
    {
        Size = size ?? throw new ArgumentNullException(nameof(size));
    }
}

public class BrowserIntersectionChangedEventArgs : EventArgs
{
    public readonly ElementReference Element;
    public readonly bool IsIntersecting;
    public readonly double IntersectionRatio;

    public BrowserIntersectionChangedEventArgs(ElementReference element, bool is_intersecting, double intersection_ratio)
    {
        Element = element;
        IsIntersecting = is_intersecting;
        IntersectionRatio = intersection_ratio;
    }
}
