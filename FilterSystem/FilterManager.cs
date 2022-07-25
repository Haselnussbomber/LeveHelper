using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using LeveHelper.Filters;

namespace LeveHelper;

public class FilterManager
{
    public Plugin Plugin { get; init; }
    public PluginUi PluginUi { get; init; }
    public FiltersState state { get; set; }
    public List<Filter> filters { get; private set; }

    public FilterManager(Plugin plugin, PluginUi pluginUi)
    {
        Plugin = plugin;
        PluginUi = pluginUi;
        state = new();
        filters = new()
        {
            new NameFilter(this),
            new StatusFilter(this),
            new LocationFilter(this),
            new ClassFilter(this),
            new LevemeteFilter(this),
        };

        Update();
    }

    public void Update()
    {
        state.leves = state.AllLeves;

        foreach (var filter in filters)
        {
            if (filter.Run())
            {
                // state changed
            }
        }

        state.leves = (state.sortColumnIndex, state.sortDirection) switch
        {
            //(0, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.value.RowId),
            (0, ImGuiSortDirection.Descending) => state.leves.OrderByDescending(item => item.leve.RowId),

            (1, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.leve.ClassJobLevel),
            (1, ImGuiSortDirection.Descending) => state.leves.OrderByDescending(item => item.leve.ClassJobLevel),

            (2, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.Name),
            (2, ImGuiSortDirection.Descending) => state.leves.OrderByDescending(item => item.Name),

            (3, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.ClassName),
            (3, ImGuiSortDirection.Descending) => state.leves.OrderByDescending(item => item.ClassName),

            (4, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.LevemeteName),
            (4, ImGuiSortDirection.Descending) => state.leves.OrderByDescending(item => item.LevemeteName),

            (5, ImGuiSortDirection.Ascending) => state.leves.OrderBy(item => item.leve.AllowanceCost),
            (5, ImGuiSortDirection.Descending) => state.leves.OrderByDescending(item => item.leve.AllowanceCost),

            _ => state.leves
        };

        state.levesArray = state.leves.ToArray();

        state.numCompletedLeves = state.leves.Where(row => row.IsComplete).Count();
        state.numTotalLeves = state.leves.Count();
        state.neededAllowances = state.leves
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

        Service.Config.Save();
        Update();
    }

    public Filter? GetFilter<T>()
    {
        return filters.Find(item => item.GetType() == typeof(T));
    }

    public void SetValue<T>(dynamic value)
    {
        GetFilter<LevemeteFilter>()?.Set(value);
        Update();
    }

    private void MoveFilter(Filter filter, int direction = 0)
    {
        var oldIndex = filters.IndexOf(filter);
        if (oldIndex == -1) return;

        filters.RemoveAt(oldIndex);
        filters.Insert(oldIndex + direction, filter);
        Update();
    }

    private void ResetOrder()
    {
        var move = (Filter filter, int newIndex) =>
        {
            var oldIndex = filters.IndexOf(filter);
            if (oldIndex == -1) return;

            filters.RemoveAt(oldIndex);
            filters.Insert(newIndex, filter);
        };

        move(GetFilter<NameFilter>()!, 0);
        move(GetFilter<StatusFilter>()!, 1);
        move(GetFilter<LocationFilter>()!, 2);
        move(GetFilter<ClassFilter>()!, 3);
        move(GetFilter<LevemeteFilter>()!, 4);
    }

    public void Draw()
    {
        ImGui.Text("Filters");

        //var cursorX = ImGui.GetCursorPosX();
        //ImGui.SetCursorPosX(cursorX + 14);

        if (ImGui.BeginTable("LeveHelper_Filters", 2/*4*/, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoSavedSettings))
        {
            for (var i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];

                filter.Draw();

                /*
                ImGui.TableNextColumn();
                if (i > 0 && ImGui.ArrowButton($"##LeveHelper_FilterManager_Filter-{i}_Up", ImGuiDir.Up))
                    MoveFilter(filter, -1);

                ImGui.TableNextColumn();
                if (i < filters.Count - 1 && ImGui.ArrowButton($"##LeveHelper_FilterManager_Filter-{i}_Down", ImGuiDir.Down))
                    MoveFilter(filter, 1);
                */
            }

            ImGui.TableNextColumn();
            ImGui.TableNextColumn();

            if (ImGui.Button("Clear Filters##LeveHelper_FilterManager_ClearFilters"))
                Reset();
            /*
            ImGui.SameLine();

            if (ImGui.Button("Reset Filter Order##LeveHelper_FilterManager_ResetFilterOrder"))
                ResetOrder();
            
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            */
        }

        ImGui.EndTable();
        //ImGui.SetCursorPosX(cursorX);
    }
}
