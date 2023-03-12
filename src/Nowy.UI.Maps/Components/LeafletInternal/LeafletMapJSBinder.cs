using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

internal class LeafletMapJSBinder : IAsyncDisposable
{
    internal readonly IJSRuntime JSRuntime;
    private Task<IJSObjectReference>? _leafletModule;
    private Task<IJSObjectReference>? _leafletMapModule;

    public LeafletMapJSBinder(IJSRuntime js)
    {
        this.JSRuntime = js;
    }

    internal async Task<IJSObjectReference> GetLeafletModule()
    {
        return await ( this._leafletModule ??=
            this.JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Nowy.Framework.Blazor/output/module-leaflet.js").AsTask() );
    }

    internal async Task<IJSObjectReference> GetLeafletMapModule()
    {
        return await ( this._leafletMapModule ??=
            this.JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Nowy.Framework.Blazor/Components/LeafletInternal/LeafletMapInternal.razor.js").AsTask() );
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (this._leafletModule is { })
        {
            IJSObjectReference m = await this._leafletModule;
            await m.DisposeAsync();
        }

        if (this._leafletMapModule is { })
        {
            IJSObjectReference m = await this._leafletMapModule;
            await m.DisposeAsync();
        }
    }
}
