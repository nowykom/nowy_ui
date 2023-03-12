namespace Nowy.UI.Common.Components;

public record NxNavigationItem(string? Title = null, string? IconUrl = null, string? LinkUrl = null);

public sealed record NxNavigationMenu(NxNavigationItem[]? Items = null) : NxNavigationItem;
