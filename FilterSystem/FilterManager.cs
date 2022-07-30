using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using LeveHelper.Filters;

namespace LeveHelper;

public class FilterManager
{
    public FiltersState state { get; set; }
    public List<Filter> filters { get; private set; }

    public FilterManager()
    {
        state = new();
        filters = new()
        {
            new NameFilter(this),
            new StatusFilter(this),
            new TypeFilter(this),
            new LocationFilter(this),
            new LevemeteFilter(this),
        };

        Update();
    }

    public void Update()
    {
        state.Leves = state.AllLeves;

        foreach (var filter in filters)
        {
            if (filter.Run())
            {
                // state changed
            }
        }

        state.Leves = (state.SortColumnIndex, state.SortDirection) switch
        {
            //(0, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.value.RowId),
            (0, ImGuiSortDirection.Descending) => state.Leves.OrderByDescending(item => item.leve.RowId),

            (1, ImGuiSortDirection.Ascending) => state.Leves.OrderBy(item => item.leve.ClassJobLevel),
            (1, ImGuiSortDirection.Descending) => state.Leves.OrderByDescending(item => item.leve.ClassJobLevel),

            (2, ImGuiSortDirection.Ascending) => state.Leves.OrderBy(item => item.Name),
            (2, ImGuiSortDirection.Descending) => state.Leves.OrderByDescending(item => item.Name),

            (3, ImGuiSortDirection.Ascending) => state.Leves.OrderBy(item => item.TypeName),
            (3, ImGuiSortDirection.Descending) => state.Leves.OrderByDescending(item => item.TypeName),

            (4, ImGuiSortDirection.Ascending) => state.Leves.OrderBy(item => item.LevemeteName),
            (4, ImGuiSortDirection.Descending) => state.Leves.OrderByDescending(item => item.LevemeteName),

            (5, ImGuiSortDirection.Ascending) => state.Leves.OrderBy(item => item.leve.AllowanceCost),
            (5, ImGuiSortDirection.Descending) => state.Leves.OrderByDescending(item => item.leve.AllowanceCost),

            _ => state.Leves
        };

        state.LevesArray = state.Leves.ToArray();

        state.NumCompletedLeves = state.Leves.Where(row => row.IsComplete).Count();
        state.NumTotalLeves = state.Leves.Count();
        state.NeededAllowances = state.Leves
            .Where(row => !row.IsComplete)
            .Select(item => (int)item.leve.AllowanceCost)
            .Aggregate(0, (total, cost) => total + cost);
    }

    public void Reset()
    {
        foreach (var filter in filters)
        {
            filter.Reset();
        }

        Configuration.Save();
        Update();
    }

    public Filter? GetFilter<T>()
    {
        return filters.Find(item => item.GetType() == typeof(T));
    }

    public void SetValue<T>(dynamic value)
    {
        GetFilter<T>()?.Set(value);
        Update();
    }

    public void Draw()
    {
        if (!ImGui.BeginTable("LeveHelper_Filters", 2, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoSavedSettings))
        {
            // ImGui.EndTable(); // LeveHelper_Filters  ??
            return;
        }

        foreach (var filter in filters)
        {
            ImGui.TableNextRow();
            filter.Draw();
        }

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();

        if (ImGui.Button("Clear Filters"))
        {
            Reset();
        }

        ImGui.EndTable(); // LeveHelper_Filters
    }
}
