namespace Nowy.UI.Grid.Components;
using Nowy.Standard;

public record NxGridField<TItem>
(
    int Index = -1,
    string? Title = null,
    string?[]? PreTitles = null,
    string?[]? PostTitles = null,
    string? Name = null,
    GridType Type = GridType.STRING,
    GridFilter Filter = GridFilter.TEXT,
    GridAccessor<TItem>? Accessor = null,
    GridMutator<TItem>? Mutator = null,
    GridPredicate<TItem>? CellVisible = null,
    GridOptions? Options = null,
    int WidthPixels = 100,
    bool IsReadonly = false,
    string? Separator = null
)
{
    private string? _cache_width_style = null;
    public string WidthStyleValue => this._cache_width_style ??= Type == GridType.BUTTON_EDIT ? "32px" : $"{WidthPixels}px";

    public string Classes => Type == GridType.BUTTON_EDIT ? "lr-grid-icon" : Type == GridType.CHECKBOX ? "lr-grid-checkbox" : "";

    public bool IsMultiple => ( Accessor?.IsMultiple ?? false ) || ( Mutator?.IsMultiple ?? false );

    public string? GetTitle(int title_index)
    {
        if (this.Title is { })
        {
            return Title;
        }

        return null;
    }

    public string? GetPreTitle(int pretitle_index)
    {
        if (this.PreTitles is { })
        {
            if (pretitle_index < PreTitles.Length)
            {
                return PreTitles[pretitle_index];
            }
        }

        return null;
    }

    public string? GetPostTitle(int posttitle_index)
    {
        if (this.PostTitles is { })
        {
            if (posttitle_index < PostTitles.Length)
            {
                return PostTitles[posttitle_index];
            }
        }

        return null;
    }

    public int GetTitleCount()
    {
        return this.Title is { } ? 1 : 0;
    }

    public int GetPreTitleCount()
    {
        return PreTitles?.Length ?? 0;
    }

    public int GetPostTitleCount()
    {
        return PostTitles?.Length ?? 0;
    }

    public string GetValue(TItem item, string? default_value = null)
    {
        if (this.IsMultiple) throw new InvalidOperationException($"{nameof(this.GetValue)} cannot be used on a field that is {nameof(this.IsMultiple)} = {this.IsMultiple}");

        string value_str = Accessor?.GetValue(item) ?? default_value ?? string.Empty;
        return value_str;
    }

    public void SetValue(TItem item, string? value = null, string? default_value = null)
    {
        if (this.IsMultiple) throw new InvalidOperationException($"{nameof(this.SetValue)} cannot be used on a field that is {nameof(this.IsMultiple)} = {this.IsMultiple}");

        Mutator?.SetValue(item, value ?? string.Empty);
    }

    public IEnumerable<string> GetValues(TItem item)
    {
        if (!this.IsMultiple) throw new InvalidOperationException($"{nameof(this.GetValues)} cannot be used on a field that is {nameof(this.IsMultiple)} = {this.IsMultiple}");

        IEnumerable<string> value_list = Accessor?.GetValues(item) ?? Array.Empty<string>();
        return value_list;
    }

    public void SetValues(TItem item, IEnumerable<string>? value = null)
    {
        if (!this.IsMultiple) throw new InvalidOperationException($"{nameof(this.SetValues)} cannot be used on a field that is {nameof(this.IsMultiple)} = {this.IsMultiple}");

        Mutator?.SetValues(item, value ?? Array.Empty<string>());
    }

    public string? GetTitle(TItem item, string? default_value = null)
    {
        if (Accessor is null)
            return null;

        string? get_title(string? v)
        {
            return Options?.GetOptionTitle(v) ?? v;
        }

        if (Accessor.IsMultiple)
        {
            IEnumerable<string> values = Accessor.GetValues(item);

            return values.Select(get_title).Join(", ");
        }
        else
        {
            string value = Accessor.GetValue(item);

            return get_title(value);
        }
    }
}

public enum GridType
{
    NONE = 0,
    STRING = 1,
    INTEGER,
    DOUBLE,
    DECIMAL,
    SELECT,
    CHECKBOX,
    BUTTON_EDIT,
}

public enum GridFilter
{
    NONE = 0,
    TEXT = 1,
    SELECT
}

public delegate TValue GridAccessor<in TItem, out TValue>(TItem item);

public delegate void GridMutator<in TItem, in TValue>(TItem item, TValue value);

public delegate bool GridPredicate<in TItem>(TItem item);

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

public sealed class GridFieldCollection<TItem>
{
    public readonly IReadOnlyList<NxGridField<TItem>> Fields;

    public GridFieldCollection(IReadOnlyList<NxGridField<TItem>> fields)
    {
        this.Fields = fields.Select((f, i) => f with { Index = i, }).ToArray();
    }

    public IReadOnlyList<IReadOnlyList<NxGridField<TItem>>> GetFieldsGroupedBySpan(int pretitle_index)
    {
        List<IReadOnlyList<NxGridField<TItem>>> ret = new();

        List<NxGridField<TItem>> current_span = new();
        foreach (NxGridField<TItem> field in this.Fields)
        {
            if (current_span.Count != 0 && current_span.Last().GetPreTitle(pretitle_index) != field.GetPreTitle(pretitle_index))
            {
                ret.Add(current_span.ToArray());
                current_span.Clear();
            }

            current_span.Add(field);
        }

        if (current_span.Count != 0)
        {
            ret.Add(current_span.ToArray());
            current_span.Clear();
        }

        return ret;
    }

    public int GetTitleCount()
    {
        int ret = 0;
        if (this.Fields.Count != 0)
        {
            if (this.Fields.Any(o => o.Title is { }))
            {
                ret = 1;
            }
        }

        return ret;
    }

    public int GetPreTitleCount()
    {
        int ret = 0;
        if (this.Fields.Count != 0)
        {
            ret = this.Fields.Select(o => o.GetPreTitleCount()).MaxBy(o => o);
        }

        return ret;
    }

    public int GetPostTitleCount()
    {
        int ret = 0;
        if (this.Fields.Count != 0)
        {
            ret = this.Fields.Select(o => o.GetPostTitleCount()).MaxBy(o => o);
        }

        return ret;
    }
}

public sealed class GridOptions
{
    private readonly IReadOnlyList<KeyValuePair<string, string?>> _options_ordered;
    private readonly IReadOnlyDictionary<string, string?> _options_as_dict;

    public GridOptions(IReadOnlyCollection<KeyValuePair<string, string?>> options)
    {
        this._options_ordered = options.ToList();
        this._options_as_dict = options.ToDictionarySafe(o => o.Key, o => o.Value);
    }

    public string? GetOptionTitle(string? value_str)
    {
        string? ret = null;
        if (this._options_as_dict.TryGetValue(value_str ?? string.Empty, out string? out_option_title))
        {
            ret = out_option_title;
        }

        ret ??= value_str ?? string.Empty;
        return ret;
    }

    public IReadOnlyList<KeyValuePair<string, string?>> GetOptions()
    {
        return this._options_ordered;
    }

    public bool HasValue(string? value_str)
    {
        return this._options_as_dict.ContainsKey(value_str ?? string.Empty);
    }
}
