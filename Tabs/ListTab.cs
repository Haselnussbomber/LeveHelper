using System;
using System.Globalization;
using System.Numerics;
using ImGuiNET;
using LeveHelper.Filters;
using static LeveHelper.ImGuiUtils;

namespace LeveHelper;

public class ListTab
{
    public PluginWindow Window { get; }

    public ListTab(PluginWindow window)
    {
        Window = window;
    }

    public void Draw()
    {
        DrawInfoBar();
        Plugin.FilterManager.Draw();
        DrawTable();
    }

    private void DrawInfoBar()
    {
        var state = Plugin.FilterManager.State;

        ImGui.Text($"Accepted Leves: {Service.GameFunctions.NumActiveLevequests}/16");
        if (ImGui.GetWindowSize().X > PluginWindow.TextWrapBreakpoint)
        {
            ImGui.SameLine();
            ImGui.Text("•");
            ImGui.SameLine();
        }

        var missing = state.NeededAllowances - Service.GameFunctions.NumAllowances;
        var missingText = missing > 0
            ? $", {missing} missing, {Math.Floor(missing / 6f)} days total"
            : "";

        ImGui.Text($"Allowances: {Service.GameFunctions.NumAllowances}/100 (need {state.NeededAllowances}{missingText}, next 3 in {Service.GameFunctions.NextAllowances - DateTime.Now:hh':'mm':'ss})");

        if (state.NumTotalLeves > 0)
        {
            if (ImGui.GetWindowSize().X > PluginWindow.TextWrapBreakpoint)
            {
                ImGui.SameLine();
                ImGui.Text("•");
                ImGui.SameLine();
            }

            var percent = (state.NumCompletedLeves / (float)state.NumTotalLeves * 100f).ToString("0.00", CultureInfo.InvariantCulture);
            ImGui.Text($"Completion: {state.NumCompletedLeves}/{state.NumTotalLeves} ({percent}%%)");
        }

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
        ImGui.Separator();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
    }

    private void DrawTable()
    {
        if (!ImGui.BeginChild("LeveHelper_TableWrapper", new Vector2(-1), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.EndChild(); // LeveHelper_TableWrapper
            return;
        }

        if (!ImGui.BeginTable("LeveHelper_Table", 5, ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable, ImGui.GetContentRegionAvail()))
        {
            //ImGui.EndTable(); // LeveHelper_Table - this throws an exception
            ImGui.EndChild(); // LeveHelper_TableWrapper
            return;
        }

        var state = Plugin.FilterManager.State;

        ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Level", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Name");
        ImGui.TableSetupColumn("Type");
        ImGui.TableSetupColumn("Levemete");
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();

        var specs = ImGui.TableGetSortSpecs();
        unsafe
        {
            if (specs.NativePtr != null && specs.SpecsDirty)
            {
                state.SortColumnIndex = specs.Specs.ColumnIndex;
                state.SortDirection = specs.Specs.SortDirection;
                specs.SpecsDirty = false;
                Plugin.FilterManager.Update();
            }
        }

        foreach (var item in state.LevesArray)
        {
            ImGui.TableNextRow();

            // Id
            ImGui.TableNextColumn();
            ImGui.Text(item.LeveId.ToString());

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Left Click: Open on GarlandTools");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                Dalamud.Utility.Util.OpenLink($"https://www.garlandtools.org/db/#leve/{item.LeveId}");
            }

            // Level
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassJobLevel.ToString());

            // Name
            ImGui.TableNextColumn();
            if (item.TownLocked)
            {
                ImGui.Text("*  ");

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip($"Only available to Characters that started in {item.TownName}.");
                }

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, default(Vector2));
                ImGui.SameLine();
                ImGui.PopStyleVar();
            }

            if (item.IsComplete)
            {
                ImGui.TextColored(ColorGreen, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("You've completed this Levequest.");
                }
            }
            else if (item.IsAccepted)
            {
                ImGui.TextColored(ColorYellow, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("You've accepted this Levequest.");
                }
            }
            else if (item.TownLocked)
            {
                ImGui.TextColored(ColorRed, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip($"Only available to characters that started in {item.TownName}.");
                }
            }
            else
            {
                ImGui.TextColored(ColorRed, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("You've not accepted or completed this Levequest.");
                }
            }

            if (item.IsCraftLeve && item.RequiredItems != null)
            {
                if (Plugin.Config.ShowInlineRecipeTree && (!Plugin.Config.ShowInlineRecipeTreeForAcceptedOnly || (Plugin.Config.ShowInlineRecipeTreeForAcceptedOnly && item.IsAccepted)))
                {
                    if (Plugin.Config.ShowInlineResultItemOnly)
                    {
                        foreach (var req in item.RequiredItems)
                            DrawItem(req.Item, req.Amount, $"InlineRecipeTree_{item.LeveId}_Item");
                    }
                    else
                    {
                        DrawIngredients($"InlineRecipeTree_{item.LeveId}", item.RequiredItems, 1);
                    }
                }
            }

            // Type
            ImGui.TableNextColumn();
            ImGui.Text(item.TypeName);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Right Click: Filter by Type");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                Plugin.FilterManager.SetValue<TypeFilter>((uint)(item.Leve?.Unknown4 ?? 0));
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
                item.Leve?.LevelLevemete.Value?.OpenMapLocation();
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                Plugin.FilterManager.SetValue<LevemeteFilter>(item.Leve?.LevelLevemete.Value?.Object ?? 0);
            }
        }

        ImGui.EndTable(); // LeveHelper_Table
        ImGui.EndChild(); // LeveHelper_TableWrapper
    }
}
