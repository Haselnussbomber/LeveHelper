using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using LeveHelper.Filters;
using LeveHelper.Sheets;
using LeveHelper.Utils;

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

    private unsafe void DrawInfoBar()
    {
        using var windowId = ImRaii.PushId("##ListTab");

        var state = Plugin.FilterManager.State;
        var questManager = QuestManager.Instance();

        ImGui.Text($"Accepted Leves: {questManager->NumAcceptedLeveQuests}/16");
        if (ImGui.GetWindowSize().X > PluginWindow.TextWrapBreakpoint)
        {
            ImGui.SameLine();
            ImGui.Text("•");
            ImGui.SameLine();
        }

        var missing = state.NeededAllowances - questManager->NumLeveAllowances;
        var missingText = missing > 0
            ? $", {missing} missing, {Math.Floor(missing / 6f)} days total"
            : "";

        ImGui.Text($"Allowances: {questManager->NumLeveAllowances}/100 (need {state.NeededAllowances}{missingText}, next 3 in {QuestManager.GetNextLeveAllowancesDateTime() - DateTime.Now:hh':'mm':'ss})");

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

        if (!ImGui.BeginTable("LeveHelper_TableV2", 5, ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable, ImGui.GetContentRegionAvail()))
        {
            //ImGui.EndTable(); // LeveHelper_Table - this throws an exception
            ImGui.EndChild(); // LeveHelper_TableWrapper
            return;
        }

        var state = Plugin.FilterManager.State;

        ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.DefaultHide, 50);
        ImGui.TableSetupColumn("Level", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Levemete", ImGuiTableColumnFlags.WidthStretch);
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
            ImGui.Text(item.RowId.ToString());

            // Level
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassJobLevel.ToString());

            // Type
            ImGui.TableNextColumn();
            if (item.TypeIcon != 0)
            {
                Window.TextureManager.GetIcon(item.TypeIcon).Draw(new(20));

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.SetTooltip(item.TypeName + "\nRight Click: Filter by Type");
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    Plugin.FilterManager.SetValue<TypeFilter>(item.LeveAssignmentType.Row);
                }
            }

            // Name
            ImGui.TableNextColumn();

            var color = Colors.Red;

            if (item.IsReadyForTurnIn)
                color = Colors.YellowGreen;
            else if (item.IsFailed)
                color = Colors.Freesia;
            else if (item.IsAccepted)
                color = Colors.Yellow;
            else if (item.IsComplete)
                color = Colors.Green;
            else if (item.TownLocked && item.Town.Row != Plugin.StartTown)
                color = Colors.Grey;

            ImGui.PushStyleColor(ImGuiCol.Text, (uint)color);
            ImGui.Selectable(item.Name);
            ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();

                if (!item.TownLocked || (item.TownLocked && item.Town.Row == Plugin.StartTown))
                {
                    if (item.IsReadyForTurnIn)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Window.TextureManager.GetIcon(71045).Draw(new(20));
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.YellowGreen);
                        ImGui.Text("Ready for turn in");
                    }
                    else if (item.IsStarted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Window.TextureManager.GetIcon(71041).Draw(new(20));
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.YellowGreen);
                        ImGui.Text("Started");
                    }
                    else if (item.IsFailed)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Window.TextureManager.GetIcon(60861).Draw(new(20));
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.TimesCircle, Colors.Freesia);
                        ImGui.Text("Failed");
                    }
                    else if (item.IsAccepted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Window.TextureManager.GetIcon(71041).Draw(new(20));
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Exclamation, Colors.Yellow);
                        ImGui.Text("Accepted");
                    }
                    else if (item.IsComplete)
                    {
                        ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.Green);
                        ImGui.Text("Complete");
                    }

                    else
                    {
                        ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Times, Colors.Red);
                        ImGui.Text("Incomplete");
                    }
                }

                if (item.TownLocked)
                {
                    ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Exclamation, Colors.Yellow);
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
                    Service.GameFunctions.AgentJournal_OpenForQuest((nint)agentJournal, (int)item.RowId, 2);
                    ImGui.SetWindowFocus(null);
                }
            }

            if (ImGui.BeginPopupContextItem($"##LeveContextMenu_{item.RowId}_Tooltip"))
            {
                var showSeparator = false;

                if (item.IsAccepted)
                {
                    if (ImGui.Selectable("Open in Journal"))
                    {
                        unsafe
                        {
                            var agentJournal = AgentModule.Instance()->GetAgentByInternalId(AgentId.Journal);
                            Service.GameFunctions.AgentJournal_OpenForQuest((nint)agentJournal, (int)item.RowId, 2);
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
                    Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#leve/{item.RowId}"));
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.BeginTooltip();
                    ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.ExternalLinkAlt, Colors.Grey);
                    ImGui.TextColored(Colors.Grey, $"https://www.garlandtools.org/db/#leve/{item.RowId}");
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
                    ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.ExternalLinkAlt, Colors.Grey);
                    ImGui.TextColored(Colors.Grey, $"https://ffxiv.consolegameswiki.com/wiki/{Uri.EscapeDataString(item.NameEn)}");
                    ImGui.EndTooltip();
                }
                */
                ImGui.EndPopup();
            }

            if (/*item.IsAccepted && */item.RequiredItems != null)
            {
                foreach (var entry in item.RequiredItems)
                {
                    if (entry.Item is Item reqItem)
                    {
                        Window.DrawItem(reqItem, entry.Amount, $"##LeveTooltip_{item.RowId}_RequiredItems_{reqItem.RowId}");
                    }
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
                item.LevelLevemete.Value?.OpenMapLocation();
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                Plugin.FilterManager.SetValue<LevemeteFilter>(item.LevelLevemete.Value?.Object ?? 0);
            }
        }

        ImGui.EndTable(); // LeveHelper_TableV2
        ImGui.EndChild(); // LeveHelper_TableWrapper
    }
}
