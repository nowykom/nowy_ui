using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A rectangular geographical area on a map.
/// <see href="https://leafletjs.com/reference-1.7.1.html#latlngbounds"/>
/// </summary>
public class LatLngBounds : InteropObject
{
    public LatLngBounds()
    {
    }

    public LatLngBounds(LatLng southwest, LatLng northeast)
    {
        this._southWest = southwest;
        this._northEast = northeast;
    }

    public LatLng _southWest { get; set; }
    public LatLng _northEast { get; set; }

    public IEnumerable<LatLng> ToLatLng()
    {
        return new List<LatLng>() { this._southWest, this._northEast };
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.latLngBounds", this._southWest.JSObjectReference, this._northEast.JSObjectReference);
    }
}
