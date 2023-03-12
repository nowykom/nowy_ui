using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Nowy.UI.Common.Services;

namespace Nowy.UI.Layout.Components;

public abstract class NxBasePage : ComponentBase, IDisposable
{
    [Inject] protected HttpClient _http { get; set; } = default!;

    [Inject] protected BrowserService _browser_service { get; set; } = default!;
    [Inject] protected ResponsiveStateService _responsive_state_service { get; set; } = default!;
    [Inject] protected NavigationManager _nav { get; set; } = default!;
    [Inject] protected IJSRuntime _js { get; set; } = default!;

    public static ResponsiveState? StaticResponsiveState { get; set; }

    public ResponsiveState? ResponsiveState { get; set; } = StaticResponsiveState;

    public bool? IsPortrait => this.ResponsiveState?.IsPortrait;
    public bool? IsLandscape => this.ResponsiveState?.IsLandscape;
    public bool? BreakpointExtraLarge => this.ResponsiveState?.BreakpointExtraLarge;
    public bool? BreakpointLarge => this.ResponsiveState?.BreakpointLarge;
    public bool? BreakpointMedium => this.ResponsiveState?.BreakpointMedium;
    public bool? BreakpointSmall => this.ResponsiveState?.BreakpointSmall;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _browser_service.WindowResized += this._handleWindowResized;
        _responsive_state_service.ResponsiveStateChanged += this._handleResponsiveStateChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        this._loadAsync().Forget();
    }

    public void Dispose()
    {
        this._dispose();
    }

    protected virtual void _dispose()
    {
        _browser_service.WindowResized -= this._handleWindowResized;
        _responsive_state_service.ResponsiveStateChanged -= this._handleResponsiveStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await this._browser_service.InitAsync();
            await this._browser_service.InitWindowResizedEventAsync();

            if (this.ResponsiveState is { })
            {
                this._handleFlavorRealizedOrChanged();
            }

            this._browser_service.SendWindowResizeAsync().Forget();
        }
    }

    protected virtual Task _loadAsync()
    {
        return Task.CompletedTask;
    }

    private void _handleWindowResized(object? sender, BrowserWindowResizedEventArgs e)
    {
    }

    private void _handleResponsiveStateChanged(object? sender, ResponsiveStateChangedEventArgs e)
    {
        if (this.ResponsiveState != e.ResponsiveState)
        {
            this.ResponsiveState = e.ResponsiveState;
            StaticResponsiveState = e.ResponsiveState;

            this._handleFlavorRealizedOrChanged();
        }
    }

    protected virtual void _handleFlavorRealizedOrChanged()
    {
    }
}
