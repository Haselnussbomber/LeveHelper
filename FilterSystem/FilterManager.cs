using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using LeveHelper.Filters;

namespace LeveHelper;

public class FilterManager
{
    public FiltersState State { get; set; }
    public List<Filter> Filters { get; private set; }

    public FilterManager()
    {
        State = new();
        Filters = new()
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
        State.Leves = State.AllLeves;

        foreach (var filter in Filters)
        {
            if (filter.Run())
            {
                // state changed
            }
        }

        State.Leves = (State.SortColumnIndex, State.SortDirection) switch
        {
            //(0, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.value.RowId),
            (0, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.Leve?.RowId),

            (1, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.Leve?.ClassJobLevel),
            (1, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.Leve?.ClassJobLevel),

            (2, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.Name),
            (2, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.Name),

            (3, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.TypeName),
            (3, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.TypeName),

            (4, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.LevemeteName),
            (4, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.LevemeteName),

            (5, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.Leve?.AllowanceCost),
            (5, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.Leve?.AllowanceCost),

            _ => State.Leves
        };

        State.LevesArray = State.Leves.ToArray();

        State.NumCompletedLeves = State.Leves.Where(row => row.IsComplete).Count();
        State.NumTotalLeves = State.Leves.Count();
        State.NeededAllowances = State.Leves
            .Where(row => !row.IsComplete && !row.IsAccepted)
            .Select(item => item.AllowanceCost)
            .Aggregate(0, (total, cost) => total + cost);
    }

    public void Reset()
    {
        foreach (var filter in Filters)
        {
            filter.Reset();
        }

        Plugin.Config.Save();
        Update();
    }

    public Filter? GetFilter<T>()
    {
        return Filters.Find(item => item.GetType() == typeof(T));
    }

    public void SetValue<T>(dynamic value)
    {
        GetFilter<T>()?.Set(value);
        Update();
    }

    public void Draw()
    {
        var someFilterSet = Filters.Any(filter => filter.HasValue());

        if (ImGui.TreeNode("LeveHelper_Filters_TreeNode", "Filters" + (someFilterSet ? " (Active)" : "")))
        {
            if (!ImGui.BeginTable("LeveHelper_Filters", 2, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.NoSavedSettings, new(450, 100)))
                return;

            foreach (var filter in Filters)
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

            ImGui.TreePop();
        }
    }
}
