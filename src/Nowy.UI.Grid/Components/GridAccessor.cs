namespace Nowy.UI.Grid.Components;

public delegate TValue GridAccessor<in TItem, out TValue>(TItem item);

public sealed class GridAccessor<TItem>
{
    private readonly GridAccessor<TItem, string>? _accessor_single;
    private readonly GridAccessor<TItem, IEnumerable<string>>? _accessor_multiple;

    public bool IsMultiple => this._accessor_multiple is { };

    public GridAccessor(GridAccessor<TItem, string>? accessor_single = null)
    {
        this._accessor_single = accessor_single;
        this._accessor_multiple = null;
    }

    public GridAccessor(GridAccessor<TItem, IEnumerable<string>>? accessor_multiple = null)
    {
        this._accessor_single = null;
        this._accessor_multiple = accessor_multiple;
    }

    public GridAccessor(GridAccessor<TItem, bool>? accessor_single = null)
    {
        this._accessor_single = o => accessor_single?.Invoke(o).ToStringInvariant() ?? string.Empty;
        this._accessor_multiple = null;
    }

    public GridAccessor(GridAccessor<TItem, long>? accessor_single = null)
    {
        this._accessor_single = o => accessor_single?.Invoke(o).ToStringInvariant() ?? string.Empty;
        this._accessor_multiple = null;
    }

    public GridAccessor(GridAccessor<TItem, double>? accessor_single = null)
    {
        this._accessor_single = o => accessor_single?.Invoke(o).ToStringInvariant() ?? string.Empty;
        this._accessor_multiple = null;
    }

    public GridAccessor(GridAccessor<TItem, decimal>? accessor_single = null)
    {
        this._accessor_single = o => accessor_single?.Invoke(o).ToStringInvariant() ?? string.Empty;
        this._accessor_multiple = null;
    }

    public string GetValue(TItem item) => ( this._accessor_single ?? throw new ArgumentNullException(nameof(this._accessor_single)) ).Invoke(item);

    public IEnumerable<string> GetValues(TItem item) => ( this._accessor_multiple ?? throw new ArgumentNullException(nameof(this._accessor_multiple)) ).Invoke(item);
}
