using System;
using System.Globalization;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using LeveHelper.Filters;

namespace LeveHelper;

public class PluginWindow : Window
{
    private readonly FilterManager filterManager = new();

    public PluginWindow() : base("LeveHelper")
    {
        base.Size = new Vector2(830, 600);
        base.SizeCondition = ImGuiCond.FirstUseEver;

        base.SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(4096, 2160)
        };
    }

    public unsafe override void Draw()
    {
        if (!Service.ClientState.IsLoggedIn) return;

        var questManager = QuestManagerHelper.Instance;
        var state = filterManager.state;

        ImGui.Text($"Accepted Leves: {questManager.NumActiveLevequests}/16");
        if (ImGui.GetWindowSize().X > 720)
        {
            ImGui.SameLine();
            ImGui.Text("•");
            ImGui.SameLine();
        }
        ImGui.Text($"Allowances: {questManager.NumAllowances}/100 (need {state.neededAllowances} over {Math.Ceiling(state.numTotalLeves / 6f)} days, next in {questManager.NextAllowances - DateTime.Now:hh':'mm':'ss})");
        if (ImGui.GetWindowSize().X > 720)
        {
            ImGui.SameLine();
            ImGui.Text("•");
            ImGui.SameLine();
        }
        var percent = (state.numCompletedLeves / (float)state.numTotalLeves * 100f).ToString("0.00", CultureInfo.InvariantCulture);
        ImGui.Text($"Completion: {state.numCompletedLeves}/{state.numTotalLeves} ({percent}%%)");

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
        ImGui.Separator();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);

        this.filterManager.Draw();

        if (!ImGui.BeginChild("LeveHelper_TableWrapper", new Vector2(-1), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.EndChild(); // LeveHelper_TableWrapper
            return;
        }

        if (!ImGui.BeginTable("LeveHelper_Table", 6, ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable, ImGui.GetContentRegionAvail()))
        {
            ImGui.EndTable(); // LeveHelper_Table
            ImGui.EndChild(); // LeveHelper_TableWrapper
            return;
        }

        ImGui.TableSetupColumn("Id");
        ImGui.TableSetupColumn("Level");
        ImGui.TableSetupColumn("Name");
        ImGui.TableSetupColumn("Class");
        ImGui.TableSetupColumn("Levemete");
        ImGui.TableSetupColumn("Allowance Cost");
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();

        var specs = ImGui.TableGetSortSpecs();
        if (specs.NativePtr != null && specs.SpecsDirty)
        {
            state.sortColumnIndex = specs.Specs.ColumnIndex;
            state.sortDirection = specs.Specs.SortDirection;
            specs.SpecsDirty = false;
            filterManager.Update();
        }

        foreach (LeveRecord item in state.levesArray)
        {
            ImGui.TableNextRow();

            // Id
            ImGui.TableNextColumn();
            ImGui.Text(item.RowId);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Left Click: Open on GarlandTools");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                Dalamud.Utility.Util.OpenLink($"https://www.garlandtools.org/db/#leve/{item.RowId}");
            }

            // Level
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassJobLevel);

            // Name
            ImGui.TableNextColumn();
            if (item.TownLocked)
            {
                ImGui.Text("*");
                ImGui.SameLine();
            }

            ImGui.TextColored(item.IsComplete ? ImGuiUtils.ColorGreen : ImGuiUtils.ColorRed, item.Name);

            if (item.TownLocked)
            {
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text($"Only available to Characters that started in {item.TownName}.");
                    ImGui.EndTooltip();
                }
            }

            // Class
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassName);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Right Click: Filter by Class");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                filterManager.SetValue<ClassFilter>((uint)item.leve.Unknown4);
            }

            // Levemete
            ImGui.TableNextColumn();
            ImGui.Text(item.LevemeteName);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Left Click: Open Map\nRight Click: Filter by Levemate");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                item.leve.LevelLevemete.Value?.OpenMapLocation();
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                filterManager.SetValue<LevemeteFilter>(item.leve.LevelLevemete.Value!.Object);
            }

            // AllowanceCost
            ImGui.TableNextColumn();
            ImGui.Text(item.leve.AllowanceCost.ToString());
        }

        ImGui.EndTable(); // LeveHelper_Table
        ImGui.EndChild(); // LeveHelper_TableWrapper
    }
}
