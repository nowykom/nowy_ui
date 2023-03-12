namespace Nowy.UI.Maps.Components;

public sealed record NxLeafletLocation(double Lat, double Lng);

public sealed record NxLeafletResolvedLocation(NxLeafletLocation Location, string? Address);

public sealed record NxLeafletClickEventArgs(NxLeafletLocation Location);

public sealed record NxLeafletInitialState(NxLeafletLocation? Center = null, int Zoom = 8);

public sealed record NxLeafletMarker(NxLeafletLocation Location, string Title);

public class NxLeafletState
{
    public NxLeafletInitialState InitialState { get; set; } = new();

    public readonly List<NxLeafletMarker> Markers = new();

    public event EventHandler<NxLeafletClickEventArgs>? Click;

    public int Zoom { get; set; }

    public void SendClick(NxLeafletClickEventArgs e)
    {
        this.Click?.Invoke(this, e);
    }
}
