using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Utils;
using ImGuiNET;
using LeveHelper.Extensions;
using LeveHelper.Filters;
using LeveHelper.Records;
using LeveHelper.Sheets;
using LeveHelper.Utils;

namespace LeveHelper;

public class ListTab
{
    private const int TextWrapBreakpoint = 820;

    private readonly WindowState _state;

    public ListTab(WindowState state)
    {
        _state = state;
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

        ImGui.TextUnformatted(t("ListTab.AcceptedLeves", questManager->NumAcceptedLeveQuests));
        if (ImGui.GetWindowSize().X > TextWrapBreakpoint)
        {
            ImGui.SameLine();
            ImGui.TextUnformatted("•");
            ImGui.SameLine();
        }

        var missing = state.NeededAllowances - questManager->NumLeveAllowances;
        var missingText = missing > 0
            ? t("ListTab.MissingText", missing, Math.Floor(missing / 6f))
            : "";

        ImGui.TextUnformatted(t(
            "ListTab.Allowances",
            questManager->NumLeveAllowances,
            state.NeededAllowances,
            missingText,
            (QuestManager.GetNextLeveAllowancesDateTime() - DateTime.Now).ToString("hh':'mm':'ss")));

        if (state.NumTotalLeves > 0)
        {
            if (ImGui.GetWindowSize().X > TextWrapBreakpoint)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted("•");
                ImGui.SameLine();
            }

            var percent = (state.NumCompletedLeves / (float)state.NumTotalLeves * 100f).ToString("0.00", CultureInfo.InvariantCulture);
            ImGui.TextUnformatted(t("ListTab.Completion", state.NumCompletedLeves, state.NumTotalLeves, percent));
        }

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
        ImGui.Separator();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
    }

    private void DrawTable()
    {
        using var wrapper = ImRaii.Child("##TableWrapper", new Vector2(-1), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        if (!wrapper.Success)
            return;

        using var table = ImRaii.Table("##Table", 5, ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable, ImGui.GetContentRegionAvail());
        if (!table.Success)
            return;

        var state = Plugin.FilterManager.State;
        var startTown = 0;
        unsafe
        {
            startTown = PlayerState.Instance()->StartTown;
        }

        ImGui.TableSetupColumn(t("ListTab.Column.Id"), ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.DefaultHide, 50);
        ImGui.TableSetupColumn(t("ListTab.Column.Level"), ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn(t("ListTab.Column.Type"), ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn(t("ListTab.Column.Name"), ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn(t("ListTab.Column.Levemete"), ImGuiTableColumnFlags.WidthStretch);
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
            ImGui.TextUnformatted(item.RowId.ToString());

            // Level
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(item.ClassJobLevel.ToString());

            // Type
            ImGui.TableNextColumn();
            if (item.TypeIcon != 0)
            {
                Service.TextureManager.GetIcon(item.TypeIcon).Draw(20);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.SetTooltip(t("ListTab.LeveType.Tooltip", item.TypeName));
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
                color = LeveHelperColors.YellowGreen;
            else if (item.IsFailed)
                color = LeveHelperColors.Freesia;
            else if (item.IsAccepted)
                color = Colors.Yellow;
            else if (item.IsComplete)
                color = Colors.Green;
            else if (item.TownLocked && item.Town.Row != startTown)
                color = Colors.Grey;

            ImGui.PushStyleColor(ImGuiCol.Text, (uint)color);
            ImGui.Selectable(item.Name);
            ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();

                if (!item.TownLocked || (item.TownLocked && item.Town.Row == startTown))
                {
                    if (item.IsReadyForTurnIn)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Service.TextureManager.GetIcon(71045).Draw(20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.YellowGreen);
                        ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.Status.ReadyForTurnIn"));
                    }
                    else if (item.IsStarted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Service.TextureManager.GetIcon(71041).Draw(20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.YellowGreen);
                        ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.Status.Started"));
                    }
                    else if (item.IsFailed)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Service.TextureManager.GetIcon(60861).Draw(20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.TimesCircle, Colors.Freesia);
                        ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.Status.Failed"));
                    }
                    else if (item.IsAccepted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        Service.TextureManager.GetIcon(71041).Draw(20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Exclamation, Colors.Yellow);
                        ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.Status.Accepted"));
                    }
                    else if (item.IsComplete)
                    {
                        ImGuiUtils.Icon(FontAwesomeIcon.Check, Colors.Green);
                        ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.Status.Completed"));
                    }

                    else
                    {
                        ImGuiUtils.Icon(FontAwesomeIcon.Times, Colors.Red);
                        ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.Status.Incomplete"));
                    }
                }

                if (item.TownLocked)
                {
                    ImGuiUtils.Icon(FontAwesomeIcon.Exclamation, Colors.Yellow);
                    ImGui.TextUnformatted(t("ListTab.Leve.Tooltip.TownLocked", item.TownName));
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
                    if (ImGui.Selectable(t("ListTab.Leve.ContextMenu.OpenInJournal")))
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

                if (ImGui.Selectable(t("ListTab.Leve.ContextMenu.OpenOnGarlandTools")))
                {
                    Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#leve/{item.RowId}"));
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.BeginTooltip();
                    ImGuiUtils.Icon(FontAwesomeIcon.ExternalLinkAlt, Colors.Grey);
                    ImGui.TextColored(Colors.Grey, $"https://www.garlandtools.org/db/#leve/{item.RowId}");
                    ImGui.EndTooltip();
                }

                ImGui.EndPopup();
            }

            if (/*item.IsAccepted && */item.RequiredItems != null)
            {
                foreach (var entry in item.RequiredItems)
                {
                    if (entry.Item is Item reqItem)
                    {
                        _state.DrawItem(reqItem, entry.Amount, $"##LeveTooltip_{item.RowId}_RequiredItems_{reqItem.RowId}");
                    }
                }
            }

            // Levemete
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(item.LevemeteName);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip(t("ListTab.LeveMete.ContextMenu.Tooltip"));
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
    }
}
