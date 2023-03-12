using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// A layer that can be added to a <see cref="Map"/>.
/// </summary>
public abstract class Layer : InteropObject
{
    /// <summary>
    /// Adds the layer to a <see cref="Map"/>.
    /// </summary>
    /// <param name="map">The <see cref="Map"/> to add the Layer to.</param>
    /// <returns>The Layer.</returns>
    public async Task<Layer> AddTo(Map map)
    {
        // Console.WriteLine($"456 a");
        if (map is null) throw new ArgumentNullException(nameof(map));
        if (this.JSBinder is null)
        {
            await this.BindJsObjectReference(map.JSBinder ?? throw new ArgumentNullException(nameof(map.JSBinder)));
        }

        // Console.WriteLine($"456 b");

        this.GuardAgainstNullBinding("Cannot add layer to map. No JavaScript binding has been set up.");
        // Console.WriteLine($"456 c");
        if (this.JSBinder is null) throw new ArgumentNullException(nameof(this.JSBinder));
        IJSObjectReference? module = await this.JSBinder.GetLeafletMapModule();
        // Console.WriteLine($"456 d");
        if (module is null) throw new ArgumentNullException(nameof(module));
        await module.InvokeVoidAsync("LeafletMap.Layer.addTo", this.JSObjectReference, map.JSObjectReference);
        // Console.WriteLine($"456 e");
        return this;
    }

    /// <summary>
    /// Removes the Layer from the <see cref="Map"/> it's currently active on.
    /// </summary>
    /// <returns>The Layer.</returns>
    public async Task<Layer> Remove()
    {
        this.GuardAgainstNullBinding("Cannot remove layer from map. No JavaScript binding has been set up.");
        IJSObjectReference? module = await this.JSBinder.GetLeafletMapModule();
        await module.InvokeVoidAsync("LeafletMap.Layer.remove", this.JSObjectReference);
        return this;
    }
}
