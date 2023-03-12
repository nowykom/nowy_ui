using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A point with a latitude and longitude.
/// <see href="https://leafletjs.com/reference-1.7.1.html#latlng"/>
/// </summary>
public class LatLng : InteropObject
{
    /// <summary>
    /// Latitude in degrees.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Lat { get; set; }


    private double _lng;

    /// <summary>
    /// Longitude in degrees.
    /// </summary>
    [JsonPropertyName("lng")]
    public double Lng
    {
        get => this._lng;
        set => this._lng = value % 360.0;
    }

    /// <summary>
    /// Constructs a LatLng
    /// </summary>
    /// <param name="lat">Latitude in degrees.</param>
    /// <param name="lng">Longitude in degrees.</param>
    public LatLng(double lat, double lng)
    {
        this.Lat = lat;
        this.Lng = lng;
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.latLng", this.Lat, this.Lng);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"({this.Lat}, {this.Lng})";
    }
}
