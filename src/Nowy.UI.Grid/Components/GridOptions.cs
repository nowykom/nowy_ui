namespace Nowy.UI.Grid.Components;

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
