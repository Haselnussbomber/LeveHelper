using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using LeveHelper.Filters;

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
        // TODO: make this a list instead, type icon in front of name, name click to open map, right click for context menu to search on garland tools
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

        ImGui.TableSetupColumn("Level", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Name");
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

            // Level
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassJobLevel.ToString());

            // Type
            ImGui.TableNextColumn();
            if (item.TypeIcon != 0)
            {
                ImGuiUtils.DrawIcon(item.TypeIcon, 20, 20);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.SetTooltip(item.TypeName + "\nRight Click: Filter by Type");
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    Plugin.FilterManager.SetValue<TypeFilter>((uint)(item.Leve?.Unknown4 ?? 0));
                }
            }

            // Name
            ImGui.TableNextColumn();

            var color = ImGuiUtils.ColorRed;
            if (item.IsComplete)
                color = ImGuiUtils.ColorGreen;
            else if (item.IsAccepted)
                color = ImGuiUtils.ColorYellow;
            else if (item.TownLocked && item.TownId != Plugin.StartTown)
                color = ImGuiUtils.ColorGrey;

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Selectable(item.Name);
            ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();

                if (!item.TownLocked || (item.TownLocked && item.TownId == Plugin.StartTown))
                {
                    if (item.IsComplete)
                    {
                        ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, ImGuiUtils.ColorGreen);
                        ImGui.Text("You've completed this Levequest.");
                    }
                    else if (item.IsAccepted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Exclamation, ImGuiUtils.ColorYellow);
                        ImGui.Text("You've accepted this Levequest.");
                    }
                    else
                    {
                        ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Times, ImGuiUtils.ColorRed);
                        ImGui.Text("You've not accepted or completed this Levequest.");
                    }
                }

                if (item.TownLocked)
                {
                    ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Exclamation, ImGuiUtils.ColorYellow);
                    ImGui.Text($"Only available to characters that started in {item.TownName}.");
                }

                /*
                {
                    var startPos = ImGui.GetCursorPos();
                    ImGuiUtils.DrawIcon(item.LeveVfxIcon, 160, 256);
                    ImGui.SetCursorPos(startPos);
                    ImGuiUtils.DrawIcon(item.LeveVfxFrameIcon, 160, 256);
                }
                */

                ImGui.EndTooltip();
            }

            if (item.IsAccepted && ImGui.IsItemClicked())
            {
                unsafe
                {
                    var agentJournal = AgentModule.Instance()->GetAgentByInternalId(AgentId.Journal);
                    Service.GameFunctions.AgentJournal_OpenForQuest((nint)agentJournal, (int)item.LeveId, 2);
                    ImGui.SetWindowFocus(null);
                }
            }

            if (ImGui.BeginPopupContextItem($"##LeveContextMenu_{item.LeveId}_Tooltip"))
            {
                var showSeparator = false;

                if (item.IsAccepted)
                {
                    if (ImGui.Selectable("Open in Journal"))
                    {
                        unsafe
                        {
                            var agentJournal = AgentModule.Instance()->GetAgentByInternalId(AgentId.Journal);
                            Service.GameFunctions.AgentJournal_OpenForQuest((nint)agentJournal, (int)item.LeveId, 2);
                            ImGui.SetWindowFocus(null);
                        }
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }

                    showSeparator = true;
                }

                if (showSeparator)
                    ImGui.Separator();

                if (ImGui.Selectable("Open on Garland Tools"))
                {
                    Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#leve/{item.LeveId}"));
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.BeginTooltip();
                    ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.ExternalLinkAlt, ImGuiUtils.ColorGrey);
                    ImGui.TextColored(ImGuiUtils.ColorGrey, $"https://www.garlandtools.org/db/#leve/{item.LeveId}");
                    ImGui.EndTooltip();
                }
                /* crashes the game?!
                if (ImGui.Selectable("Open on Final Fantasy XIV A Realm Reborn Wiki"))
                {
                    Task.Run(() => Util.OpenLink($"https://ffxiv.consolegameswiki.com/wiki/{Uri.EscapeDataString(item.NameEn)}"));
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.BeginTooltip();
                    ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.ExternalLinkAlt, ImGuiUtils.ColorGrey);
                    ImGui.TextColored(ImGuiUtils.ColorGrey, $"https://ffxiv.consolegameswiki.com/wiki/{Uri.EscapeDataString(item.NameEn)}");
                    ImGui.EndTooltip();
                }
                */
                ImGui.EndPopup();
            }

            if (item.IsCraftLeve && item.IsAccepted && item.RequiredItems != null)
            {
                foreach (var entry in item.RequiredItems)
                {
                    ImGuiUtils.DrawItem(entry.Item, entry.Amount, $"##LeveTooltip_{item.LeveId}_RequiredItems_{entry.Item.ItemId}");
                }
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
