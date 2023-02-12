using System;
using System.Globalization;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using LeveHelper.Filters;

namespace LeveHelper;

public class PluginWindow : Window
{
    private const int TextWrapBreakpoint = 865;

    public Vector2 MyPosition { get; private set; }
    public Vector2 MySize { get; private set; }

    public PluginWindow() : base("LeveHelper")
    {
        base.Size = new Vector2(830, 600);
        base.SizeCondition = ImGuiCond.FirstUseEver;
        base.SizeConstraints = new()
        {
            MinimumSize = new Vector2(490, 400),
            MaximumSize = new Vector2(4096, 2160)
        };
    }

    public override void OnOpen()
    {
        Plugin.FilterManager ??= new();
    }

    public override void OnClose()
    {
        Plugin.ConfigWindow.IsOpen = false;
    }

    public override bool DrawConditions()
    {
        return Service.ClientState.IsLoggedIn;
    }

    public override void Draw()
    {
        MyPosition = ImGui.GetWindowPos();
        MySize = ImGui.GetWindowSize();

        DrawButtons();
        DrawInfoBar();

        Plugin.FilterManager.Draw();

        DrawTable();
    }

    private void DrawButtons()
    {
        var cursorStartPos = ImGui.GetCursorPos();

        ImGui.SetCursorPosY(cursorStartPos.Y - 3);
        ImGui.SetCursorPosX(ImGui.GetWindowSize().X - 30);

        if (ImGuiComponents.IconButton(FontAwesomeIcon.Cog))
        {
            if (Plugin.CraftingHelperWindow != null && Plugin.CraftingHelperWindow.IsOpen)
                Plugin.CraftingHelperWindow.IsOpen = false;

            Plugin.ConfigWindow.IsOpen = true;
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip((Plugin.ConfigWindow.IsOpen ? "Close" : "Open") + " Configuration");
        }

        ImGui.SetCursorPosY(cursorStartPos.Y - 3);
        ImGui.SetCursorPosX(ImGui.GetWindowSize().X - 30 - 30);

        if (ImGuiComponents.IconButton(FontAwesomeIcon.ShoppingCart))
        {
            Plugin.ConfigWindow.IsOpen = false;

            if (Plugin.CraftingHelperWindow == null)
            {
                Plugin.CraftingHelperWindow = new CraftingHelperWindow();
                Plugin.WindowSystem.AddWindow(Plugin.CraftingHelperWindow);
            }

            Plugin.CraftingHelperWindow.IsOpen = true;
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip((Plugin.ConfigWindow.IsOpen ? "Close" : "Open") + " Crafting Helper");
        }

        ImGui.SetCursorPos(cursorStartPos);
    }

    private void DrawInfoBar()
    {
        var state = Plugin.FilterManager.State;

        ImGui.Text($"Accepted Leves: {Service.GameFunctions.NumActiveLevequests}/16");
        if (ImGui.GetWindowSize().X > TextWrapBreakpoint)
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
            if (ImGui.GetWindowSize().X > TextWrapBreakpoint)
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

        if (!ImGui.BeginTable("LeveHelper_Table", 6, ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable, ImGui.GetContentRegionAvail()))
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
        ImGui.TableSetupColumn("Allowance Cost");
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
                ImGui.TextColored(ImGuiUtils.ColorGreen, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("You've completed this Levequest.");
                }
            }
            else if (item.IsAccepted)
            {
                ImGui.TextColored(ImGuiUtils.ColorYellow, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("You've accepted this Levequest.");
                }
            }
            else if (item.TownLocked)
            {
                ImGui.TextColored(ImGuiUtils.ColorRed, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip($"Only available to Characters that started in {item.TownName}.");
                }
            }
            else
            {
                ImGui.TextColored(ImGuiUtils.ColorRed, item.Name);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("You've not accepted or completed this Levequest.");
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

            // AllowanceCost
            ImGui.TableNextColumn();
            ImGui.Text(item.Leve?.AllowanceCost.ToString());
        }

        ImGui.EndTable(); // LeveHelper_Table
        ImGui.EndChild(); // LeveHelper_TableWrapper
    }
}
