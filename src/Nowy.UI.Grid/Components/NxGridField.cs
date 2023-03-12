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



public delegate bool GridPredicate<in TItem>(TItem item);
