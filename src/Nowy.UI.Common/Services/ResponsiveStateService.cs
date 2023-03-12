using Microsoft.JSInterop;

namespace Nowy.UI.Common.Services;

public class ResponsiveStateService
{
    public event EventHandler<ResponsiveStateChangedEventArgs>? ResponsiveStateChanged;

    private readonly IJSInProcessRuntime _js;
    private readonly BrowserService browser_service;
    private IJSInProcessObjectReference? _module;

    private bool _window_resize_is_initialized;

    public ResponsiveStateService(IJSRuntime js, BrowserService browser_service)
    {
        this._js = ( js as IJSInProcessRuntime ) ?? throw new ArgumentNullException(nameof(js));

        this.InitAsync().Forget();

        browser_service.WindowResized += this._handleWindowResized;
    }

    public async Task InitAsync()
    {
        this._module ??= await this._js.InvokeAsync<IJSInProcessObjectReference>("import", "./_content/Nowy.Framework.Blazor/output/Services/BrowserService.js");
    }

    private void _handleWindowResized(object? sender, Common.Services.BrowserWindowResizedEventArgs e)
    {
        int w = e.Size.Width;
        int h = e.Size.Height;

        bool is_landscape;
        bool is_portrait;

        if (w > h * 0.9)
        {
            is_landscape = true;
            is_portrait = false;
        }
        else
        {
            is_landscape = false;
            is_portrait = true;
        }

        bool breakpoint_extra_large = false;
        bool breakpoint_large = false;
        bool breakpoint_medium = false;
        bool breakpoint_small = false;
        if (w >= 1366)
            breakpoint_extra_large = true;
        if (w >= 1024)
            breakpoint_large = true;
        if (w >= 640)
            breakpoint_medium = true;
        if (w >= 480)
            breakpoint_small = true;

        ResponsiveState responsive_state = new(
            IsPortrait: is_portrait,
            IsLandscape: is_landscape,
            BreakpointExtraLarge: breakpoint_extra_large,
            BreakpointLarge: breakpoint_large,
            BreakpointMedium: breakpoint_medium,
            BreakpointSmall: breakpoint_small
        );

        this.ResponsiveStateChanged?.Invoke(this, new ResponsiveStateChangedEventArgs(responsive_state));
    }
}

public record ResponsiveState(
    bool IsPortrait,
    bool IsLandscape,
    bool BreakpointExtraLarge,
    bool BreakpointLarge,
    bool BreakpointMedium,
    bool BreakpointSmall
);

public class ResponsiveStateChangedEventArgs : EventArgs
{
    public readonly ResponsiveState ResponsiveState;

    public ResponsiveStateChangedEventArgs(ResponsiveState responsive_state)
    {
        this.ResponsiveState = responsive_state ?? throw new ArgumentNullException(nameof(responsive_state));
    }
}
