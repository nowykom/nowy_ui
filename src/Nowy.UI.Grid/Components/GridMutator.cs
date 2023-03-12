namespace Nowy.UI.Grid.Components;

public delegate void GridMutator<in TItem, in TValue>(TItem item, TValue value);

public sealed class GridMutator<TItem>
{
    private readonly GridMutator<TItem, string>? _mutator_single;
    private readonly GridMutator<TItem, IEnumerable<string>>? _mutator_multiple;

    public bool IsMultiple => this._mutator_multiple is { };

    public GridMutator(GridMutator<TItem, string>? mutator_single = null)
    {
        this._mutator_single = mutator_single;
        this._mutator_multiple = null;
    }

    public GridMutator(GridMutator<TItem, IEnumerable<string>>? mutator_multiple = null)
    {
        this._mutator_single = null;
        this._mutator_multiple = mutator_multiple;
    }

    public GridMutator(GridMutator<TItem, bool>? mutator_single = null)
    {
        this._mutator_single = (o, v) => mutator_single?.Invoke(o, v.ToStringInvariant() == "True");
        this._mutator_multiple = null;
    }

    public GridMutator(GridMutator<TItem, long>? mutator_single = null)
    {
        this._mutator_single = (o, v) => mutator_single?.Invoke(o, v.ToLong());
        this._mutator_multiple = null;
    }

    public GridMutator(GridMutator<TItem, double>? mutator_single = null)
    {
        this._mutator_single = (o, v) => mutator_single?.Invoke(o, v.ToDouble());
        this._mutator_multiple = null;
    }

    public GridMutator(GridMutator<TItem, decimal>? mutator_single = null)
    {
        this._mutator_single = (o, v) => mutator_single?.Invoke(o, v.ToDecimal());
        this._mutator_multiple = null;
    }

    public GridMutator(GridMutator<TItem, List<string>>? mutator_multiple = null)
    {
        this._mutator_single = null;
        this._mutator_multiple = (o, v) => mutator_multiple?.Invoke(o, v.ToList());
    }

    public GridMutator(GridMutator<TItem, IReadOnlyList<string>>? mutator_multiple = null)
    {
        this._mutator_single = null;
        this._mutator_multiple = (o, v) => mutator_multiple?.Invoke(o, v.ToArray());
    }

    public GridMutator(GridMutator<TItem, string[]>? mutator_multiple = null)
    {
        this._mutator_single = null;
        this._mutator_multiple = (o, v) => mutator_multiple?.Invoke(o, v.ToArray());
    }

    public void SetValue(TItem item, string value) => ( this._mutator_single ?? throw new ArgumentNullException(nameof(this._mutator_single)) ).Invoke(item, value);

    public void SetValues(TItem item, IEnumerable<string> value) =>
        ( this._mutator_multiple ?? throw new ArgumentNullException(nameof(this._mutator_multiple)) ).Invoke(item, value);
}
