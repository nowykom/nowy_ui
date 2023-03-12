using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

///<summary>
/// Represents an icon to provide when creating a marker.
///</summary>
public class Icon : InteractiveLayer
{
    // /// <summary>
    // /// The ID of the HTML element the icon will be rendered in.
    // /// </summary>
    // public string ElementId { get; }

    /// <summary>
    /// The <see cref="IconOptions"/> used to create the Icon.
    /// </summary>
    [JsonPropertyName("options")]
    public IconOptions Options { get; protected set; }

    /// <summary>
    /// Constructs an Icon.
    /// </summary>        
    /// <param name="options">The <see cref="IconOptions"/> used to create the Icon.</param>
    public Icon(IconOptions options)
    {
        this.Options = options;
    }

    /// <summary>
    /// <param name="marker">The <see cref="Marker"/> to which to add the Icon.</param>        
    /// </summary>
    public async Task<Layer> AddTo(Marker marker)
    {
        if (this.JSBinder is null)
        {
            await this.BindJsObjectReference(marker.JSBinder);
        }

        this.GuardAgainstNullBinding("Cannot add layer to map. No JavaScript binding has been set up.");
        IJSObjectReference? module = await this.JSBinder.GetLeafletMapModule();
        await module.InvokeVoidAsync("LeafletMap.Marker.setIcon", this.JSObjectReference, marker.JSObjectReference);

        return this;
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.icon", this.Options);
    }
}
