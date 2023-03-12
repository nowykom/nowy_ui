namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// Represents a mouse interaction event
/// </summary>
public class LeafletZoomEventArgs : LeafletEventArgs
{
    public int Zoom { get; set; }
}
