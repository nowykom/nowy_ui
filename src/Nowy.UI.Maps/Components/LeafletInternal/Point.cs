using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A point with x and y coordinates in pixels.
/// </summary>
public class Point : InteropObject
{
    /// <summary>
    /// The x coordinate in pixels.
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; }

    /// <summary>
    /// The y corrdinate in pixels.
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }

    /// <summary>
    /// Flag indicating whether coordinate values should be rounded.
    /// </summary>
    [JsonPropertyName("round")]
    public bool Round { get; set; }

    /// <summary>
    /// Constructs a point
    /// </summary>
    /// <param name="x">The x coordinate in pixels.</param>
    /// <param name="y">The y corrdinate in pixels.</param>
    /// <param name="round">Flag indicating whether coordinate values should be rounded.</param>
    public Point(double x, double y, bool round = false)
    {
        this.X = x;
        this.Y = y;
        this.Round = round;
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.point", this.X, this.Y, this.Round);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"({this.X}, {this.Y})";
    }
}
