using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Graphics;
using HaselCommon.Gui;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Caches;
using LeveHelper.Filters;
using LeveHelper.Records;
using LeveHelper.Utils;
using Lumina.Excel.Sheets;

namespace LeveHelper;

public class ListTab(
    WindowState WindowState,
    FilterManager FilterManager,
    TextureService TextureService,
    TextService TextService,
    ExcelService ExcelService,
    MapService MapService,
    LeveService LeveService,
    LeveIssuerCache LeveIssuerCache,
    ItemService ItemService)
{
    private const int TextWrapBreakpoint = 820;

    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem(TextService.Translate("Tabs.Levequest"));
        if (!tab) return;

        window.RespectCloseHotkey = true;

        DrawInfoBar();
        FilterManager.Draw();
        DrawTable();
    }

    private void DrawInfoBar()
    {
        using var windowId = ImRaii.PushId("##ListTab");

        var state = FilterManager.State;
        var numLeveAllowances = LeveService.GetNumLeveAllowances();

        TextService.Draw("ListTab.AcceptedLeves", LeveService.GetNumAcceptedLeveQuests());
        if (ImGui.GetWindowSize().X > TextWrapBreakpoint)
        {
            ImGui.SameLine();
            ImGui.TextUnformatted("•");
            ImGui.SameLine();
        }

        var missing = state.NeededAllowances - numLeveAllowances;
        var missingText = missing > 0
            ? TextService.Translate("ListTab.MissingText", missing, Math.Floor(missing / 6f))
            : "";

        ImGui.TextUnformatted(TextService.Translate(
            "ListTab.Allowances",
            numLeveAllowances,
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
            TextService.Draw("ListTab.Completion", state.NumCompletedLeves, state.NumTotalLeves, percent);
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

        var state = FilterManager.State;
        var startTown = 0;
        unsafe
        {
            startTown = PlayerState.Instance()->StartTown;
        }

        ImGui.TableSetupColumn(TextService.Translate("ListTab.Column.Id"), ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.DefaultHide, 50);
        ImGui.TableSetupColumn(TextService.Translate("ListTab.Column.Level"), ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn(TextService.Translate("ListTab.Column.Type"), ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn(TextService.Translate("ListTab.Column.Name"), ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn(TextService.Translate("ListTab.Column.Levemete"), ImGuiTableColumnFlags.WidthStretch);
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
                FilterManager.Update();
            }
        }

        foreach (var leve in state.LevesArray)
        {
            var isTownLocked = LeveService.IsTownLocked(leve);
            var isReadyForTurnIn = LeveService.IsReadyForTurnIn(leve);
            var isFailed = LeveService.IsFailed(leve);
            var isAccepted = LeveService.IsAccepted(leve);
            var isStarted = LeveService.IsStarted(leve);
            var isComplete = LeveService.IsComplete(leve);

            ImGui.TableNextRow();

            // Id
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(leve.RowId.ToString());

            // Level
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(leve.ClassJobLevel.ToString());

            // Type
            ImGui.TableNextColumn();
            var typeIcon = leve.LeveAssignmentType.ValueNullable?.Icon ?? 0;
            if (typeIcon != 0)
            {
                TextureService.DrawIcon(typeIcon, 20);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.SetTooltip(TextService.Translate("ListTab.LeveType.Tooltip", leve.LeveAssignmentType.ValueNullable?.Name ?? string.Empty));
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    FilterManager.GetFilter<TypeFilter>().Set(leve.LeveAssignmentType.RowId);
                    FilterManager.Update();
                }
            }

            // Name
            ImGui.TableNextColumn();

            var color = Color.Red;

            if (isReadyForTurnIn)
                color = LeveHelperColors.YellowGreen;
            else if (isFailed)
                color = LeveHelperColors.Freesia;
            else if (isAccepted)
                color = Color.Yellow;
            else if (isComplete)
                color = Color.Green;
            else if (isTownLocked && leve.Town.RowId != startTown)
                color = Color.Grey;

            using (ImRaii.PushColor(ImGuiCol.Text, (uint)color))
                ImGui.Selectable(LeveService.GetLeveName(leve));

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();

                if (!isTownLocked || (isTownLocked && leve.Town.RowId == startTown))
                {
                    if (isReadyForTurnIn)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        TextureService.DrawIcon(71045, 20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.YellowGreen);
                        TextService.Draw("ListTab.Leve.Tooltip.Status.ReadyForTurnIn");
                    }
                    else if (isStarted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        TextureService.DrawIcon(71041, 20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Check, Colors.YellowGreen);
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Started");
                    }
                    else if (isFailed)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        TextureService.DrawIcon(60861, 20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.TimesCircle, Colors.Freesia);
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Failed");
                    }
                    else if (isAccepted)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        TextureService.DrawIcon(71041, 20);
                        ImGui.SameLine(0, 0);
                        //ImGuiUtils.DrawFontAwesomeIcon(FontAwesomeIcon.Exclamation, Colors.Yellow);
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Accepted");
                    }
                    else if (isComplete)
                    {
                        ImGuiUtils.Icon(FontAwesomeIcon.Check, Color.Green);
                        ImGui.SameLine();
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Completed");
                    }

                    else
                    {
                        ImGuiUtils.Icon(FontAwesomeIcon.Times, Color.Red);
                        ImGui.SameLine();
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Incomplete");
                    }
                }

                if (isTownLocked)
                {
                    ImGuiUtils.Icon(FontAwesomeIcon.Exclamation, Color.Yellow);
                    TextService.Draw("ListTab.Leve.Tooltip.TownLocked", leve.Town.ValueNullable?.Name.ExtractText() ?? string.Empty);
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

            if (isAccepted && ImGui.IsItemClicked())
            {
                unsafe
                {
                    AgentQuestJournal.Instance()->OpenForQuest(leve.RowId, 2);
                    ImGui.SetWindowFocus(null);
                }
            }

            if (ImGui.BeginPopupContextItem($"##LeveContextMenu_{leve.RowId}_Tooltip"))
            {
                var showSeparator = false;

                if (isAccepted)
                {
                    if (ImGui.Selectable(TextService.Translate("ListTab.Leve.ContextMenu.OpenInJournal")))
                    {
                        unsafe
                        {
                            AgentQuestJournal.Instance()->OpenForQuest(leve.RowId, 2);
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

                if (ImGui.Selectable(TextService.Translate("ListTab.Leve.ContextMenu.OpenOnGarlandTools")))
                {
                    Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#leve/{leve.RowId}"));
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.BeginTooltip();
                    ImGuiUtils.Icon(FontAwesomeIcon.ExternalLinkAlt, Color.Grey);
                    ImGui.TextColored(Color.Grey, $"https://www.garlandtools.org/db/#leve/{leve.RowId}");
                    ImGui.EndTooltip();
                }
                ImGui.EndPopup();
            }

            var requiredItems = LeveService.GetRequiredItems(leve);
            if (requiredItems.Length != 0)
            {
                foreach (var entry in requiredItems)
                {
                    WindowState.DrawItem(entry.Item, entry.Amount, $"##LeveTooltip_{leve.RowId}_RequiredItems_{entry.Item.RowId}");
                }
            }

            // Issuer
            ImGui.TableNextColumn();
            if (LeveIssuerCache.TryGetValue(leve.RowId, out var issuers))
            {
                for (var i = 0; i < issuers.Length; i++)
                {
                    var issuer = issuers[i];

                    ImGui.TextUnformatted(TextService.GetENpcResidentName(issuer.RowId));

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        ImGui.SetTooltip(TextService.Translate("ListTab.Levemete.ContextMenu.Tooltip"));
                    }

                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left) && ExcelService.TryFindRow<Level>(row => row.Object.RowId == issuer.RowId, out var level))
                    {
                        MapService.OpenMap(level);
                    }

                    if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        FilterManager.GetFilter<LevemeteFilter>().Set(issuer.RowId);
                        FilterManager.Update();
                    }

                    if (i < issuers.Length - 1)
                    {
                        ImGui.SameLine(0, 0);
                        ImGui.TextUnformatted(",");
                        ImGuiUtils.SameLineSpace();
                    }
                }
            }
        }
    }
}
