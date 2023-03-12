using System.Text.Json.Serialization;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// The options used when creating a <see cref="Layer"/>.
/// </summary>
public class LayerOptions
{
    /// <summary>
    /// The string shown in the attribution control.
    /// May be required to show, e.g., tile provider's copyright message.
    /// </summary>
    [JsonPropertyName("attribution")]
    public string Attribution { get; set; }
}
