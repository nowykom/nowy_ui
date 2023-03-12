using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A vector line overlay <see cref="Layer"/>.
/// <see href="https://leafletjs.com/reference-1.7.1.html#polyline"/>
/// </summary>
public class Polyline : Path
{
    /// <summary>
    /// An array of points defining the shape.
    /// </summary>
    [JsonIgnore]
    public IEnumerable<LatLng> LatLngs { get; }

    /// <summary>
    /// The <see cref="PolylineOptions"/> used to define the Polyline.
    /// </summary>
    [JsonIgnore]
    public PolylineOptions Options { get; }

    /// <summary>
    /// Constructs a Polyline.
    /// </summary>
    /// <param name="latLngs">An array of points defining the shape.</param>
    /// <param name="options">The <see cref="PolylineOptions"/> used to define the polyline.</param>
    public Polyline(IEnumerable<LatLng> latLngs, PolylineOptions options)
    {
        this.LatLngs = latLngs;
        this.Options = options;
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.polyline", this.LatLngs.ToArray(), this.Options);
    }

    /// <summary>
    /// Adds a point to the Polyline
    /// </summary>
    /// <param name="latLng">The point to add to the Polyline.</param>
    /// <returns>The Polyline.</returns>
    public async Task<Polyline> AddLatLng(LatLng latLng)
    {
        this.GuardAgainstNullBinding("Cannot remove layer from map. No JavaScript binding has been set up.");
        IJSObjectReference? module = await this.JSBinder.GetLeafletMapModule();
        await module.InvokeVoidAsync("LeafletMap.Polyline.addLatLng", this.JSObjectReference, latLng);
        return this;
    }
}
