using Microsoft.AspNetCore.Components;
using Nowy.UI.Common.Services;

namespace Nowy.UI.Layout.Components;

public class NxBaseMainLayout : LayoutComponentBase, IDisposable
{
    [Inject] protected BrowserService _browser_service { get; set; } = default!;
    [Inject] protected ResponsiveStateService _responsive_state_service { get; set; } = default!;

    private NxPageSettings? _page_settings;

    protected RenderFragment? SideMenuContent => this._page_settings?.SideMenuContent;
    protected bool HasPadding => this._page_settings?.HasPadding ?? true;
    protected bool HasCenteredContainer => this._page_settings?.HasCenteredContainer ?? true;

    public ResponsiveState? ResponsiveState { get; set; }

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

    public void Dispose()
    {
        this._dispose();
    }

    protected virtual void _dispose()
    {
        _browser_service.WindowResized -= this._handleWindowResized;
        _responsive_state_service.ResponsiveStateChanged -= this._handleResponsiveStateChanged;
    }

    private void _handleWindowResized(object? sender, BrowserWindowResizedEventArgs e)
    {
    }

    private void _handleResponsiveStateChanged(object? sender, ResponsiveStateChangedEventArgs e)
    {
        if (this.ResponsiveState != e.ResponsiveState)
        {
            this.ResponsiveState = e.ResponsiveState;

            this.StateHasChanged();

            this._handleFlavorRealizedOrChanged();
        }
    }

    protected virtual void _handleFlavorRealizedOrChanged()
    {
    }

    public void SetPageSettings(NxPageSettings? page_settings)
    {
        this._page_settings = page_settings;
        this.SendStateHasChanged();
    }

    public void SendStateHasChanged()
    {
        this.StateHasChanged();
    }
}
