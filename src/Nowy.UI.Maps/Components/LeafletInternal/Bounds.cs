using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A rectangular area in pixel coordinates.
/// </summary>
public class Bounds : InteropObject
{
    /// <summary>
    /// The top left corner of the bounds.
    /// </summary>
    [JsonPropertyName("min")]
    public Point Min { get; set; }

    /// <summary>
    /// The bottom right corner of the bounds.
    /// </summary>
    [JsonPropertyName("max")]
    public Point Max { get; set; }

    /// <summary>
    /// Constructs a Bounds instance.
    /// </summary>
    /// <param name="min">The top left corner of the bounds.</param>
    /// <param name="max">The bottom right corner of the bounds.</param>        
    public Bounds(Point min, Point max)
    {
        this.Min = min;
        this.Max = max;
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        if (this.JSBinder is null) throw new ArgumentNullException(nameof(this.JSBinder));
        
        if (this.Min.JSBinder is null)
        {
            await this.Min.BindJsObjectReference(this.JSBinder);
        }

        this.Min.GuardAgainstNullBinding("Cannot create Bounds object. No JavaScript binding has been set up for the Min property.");
        if (this.Max.JSBinder is null)
        {
            await this.Max.BindJsObjectReference(this.JSBinder);
        }

        this.Max.GuardAgainstNullBinding("Cannot create Bounds object. No JavaScript binding has been set up for the Max property.");
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.bounds", this.Min.JSObjectReference, this.Max.JSObjectReference);
    }

    /// <inheritsdoc/>
    public override string ToString()
    {
        return $"[{this.Min}, {this.Max}]";
    }
}
