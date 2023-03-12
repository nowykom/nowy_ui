using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A clickable, draggable icon that can be added to a <see cref="Map"/>
/// <see href="https://leafletjs.com/reference-1.7.1.html#marker"/>
/// </summary>
public class Marker : InteractiveLayer
{
    /// <summary>
    /// The initial position of the marker.
    /// </summary>
    [JsonIgnore]
    public LatLng LatLng { get; private set; }

    /// <summary>
    /// The <see cref="MarkerOptions"/> used to create the marker.
    /// </summary>
    [JsonIgnore]
    public MarkerOptions Options { get; }

    /// <summary>
    /// Constructs a marker
    /// </summary>
    /// <param name="latlng">The initial position of the marker.</param>
    /// <param name="options">The <see cref="MarkerOptions"/> used to create the marker.</param>
    public Marker(LatLng latlng, MarkerOptions options)
    {
        this.LatLng = latlng;
        this.Options = options;
    }

    /// <summary>
    /// Changes the marker position to the given point.
    /// </summary>
    /// <param name="latlng">Coordinates of the new position of the marker.</param>
    public async Task SetLatLng(LatLng latlng)
    {
        this.GuardAgainstNullBinding("Cannot set marker position. No JavaScript binding has been set up.");
        IJSObjectReference? module = await this.JSBinder.GetLeafletMapModule();
        await module.InvokeVoidAsync("LeafletMap.Marker.setLatLng", latlng, this.JSObjectReference);
        this.LatLng = latlng;
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        // Console.WriteLine($"888 a {JSBinder}");
        // Console.WriteLine($"888 b {JSBinder?.JSRuntime}");
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        ValueTask<IJSObjectReference> task = module.InvokeAsync<IJSObjectReference>("L.marker", this.LatLng, this.Options);
        // Console.WriteLine($"888 c {task}");
        IJSObjectReference ret = await task;
        // Console.WriteLine($"888 ret {ret}");
        return ret;
    }
}
