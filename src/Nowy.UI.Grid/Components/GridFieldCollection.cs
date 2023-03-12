namespace Nowy.UI.Grid.Components;

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
