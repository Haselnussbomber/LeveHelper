using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility.Raii;
using HaselCommon.Extensions;
using HaselCommon.Utils;
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
            (0, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.RowId),
            (0, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.RowId),

            (1, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.ClassJobLevel),
            (1, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.ClassJobLevel),

            (2, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.TypeName),
            (2, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.TypeName),

            (3, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.Name),
            (3, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.Name),

            (4, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(item => item.Issuers.FirstOrNull()?.Name ?? string.Empty),
            (4, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(item => item.Issuers.FirstOrNull()?.Name ?? string.Empty),

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

    public void Reload()
    {
        State.Reload();

        foreach (var filter in Filters)
        {
            filter.Reload();
        }

        Update();
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
        using var id = ImRaii.PushId("Filters");

        var someFilterSet = Filters.Any(filter => filter.HasValue());

        using var treeNodeColor = someFilterSet ? ImRaii.PushColor(ImGuiCol.Text, (uint)Colors.Green) : null;
        using var treeNode = ImRaii.TreeNode(t("FilterManager.Title"), ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.FramePadding);
        if (!treeNode.Success)
            return;
        treeNodeColor?.Dispose();

        using var table = ImRaii.Table("Filters", 2, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.NoSavedSettings, new(-1, 100));
        if (!table.Success)
            return;

        ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed, 100);
        ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.WidthStretch);

        foreach (var filter in Filters)
        {
            ImGui.TableNextRow();
            filter.Draw();
        }

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();

        if (ImGui.Button(t("FilterManager.ClearFilters")))
        {
            Reset();
        }
    }
}
