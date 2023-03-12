using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// Represents a lightweight icon for markers that uses a simple div element instead of an image. Inherits from Icon but ignores the iconUrl and shadow options.
/// </summary>
public class DivIcon : Icon
{
    /// <summary>
    /// Constructs a DivIcon.
    /// </summary>        
    /// <param name="options">The <see cref="DivIconOptions"/> used to create the DivIcon.</param>
    public DivIcon(DivIconOptions options)
        : base(options)
    {
    }

    /// <inheritdoc/>
    protected override async Task<IJSObjectReference> _createJsObjectRef()
    {
        if (this.JSBinder is null) throw new ArgumentNullException(nameof(this.JSBinder));

        IJSObjectReference? module = await this.JSBinder.GetLeafletModule();
        return await module.InvokeAsync<IJSObjectReference>("L.divIcon", this.Options);
    }
}
