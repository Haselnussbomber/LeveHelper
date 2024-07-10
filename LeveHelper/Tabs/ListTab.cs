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
using HaselCommon.Extensions;
using HaselCommon.Services;
using HaselCommon.Utils;
using ImGuiNET;
using LeveHelper.Caches;
using LeveHelper.Filters;
using LeveHelper.Records;
using LeveHelper.Sheets;
using LeveHelper.Utils;
using Lumina.Excel.GeneratedSheets;

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
    LeveRequiredItemsCache LeveRequiredItemsCache)
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

        foreach (var item in state.LevesArray)
        {
            var isTownLocked = LeveService.IsTownLocked(item);
            var isReadyForTurnIn = LeveService.IsReadyForTurnIn(item);
            var isFailed = LeveService.IsFailed(item);
            var isAccepted = LeveService.IsAccepted(item);
            var isStarted = LeveService.IsStarted(item);
            var isComplete = LeveService.IsComplete(item);

            ImGui.TableNextRow();

            // Id
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(item.RowId.ToString());

            // Level
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(item.ClassJobLevel.ToString());

            // Type
            ImGui.TableNextColumn();
            var typeIcon = item.LeveAssignmentType.Value?.Icon ?? 0;
            if (typeIcon != 0)
            {
                TextureService.DrawIcon(typeIcon, 20);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    ImGui.SetTooltip(TextService.Translate("ListTab.LeveType.Tooltip", item.LeveAssignmentType.Value?.Name ?? ""));
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    FilterManager.SetValue<TypeFilter>(item.LeveAssignmentType.Row);
                }
            }

            // Name
            ImGui.TableNextColumn();

            var color = Colors.Red;

            if (isReadyForTurnIn)
                color = LeveHelperColors.YellowGreen;
            else if (isFailed)
                color = LeveHelperColors.Freesia;
            else if (isAccepted)
                color = Colors.Yellow;
            else if (isComplete)
                color = Colors.Green;
            else if (isTownLocked && item.Town.Row != startTown)
                color = Colors.Grey;

            using (ImRaii.PushColor(ImGuiCol.Text, (uint)color))
                ImGui.Selectable(item.Name);

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();

                if (!isTownLocked || (isTownLocked && item.Town.Row == startTown))
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
                        ImGuiUtils.Icon(FontAwesomeIcon.Check, Colors.Green);
                        ImGui.SameLine();
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Completed");
                    }

                    else
                    {
                        ImGuiUtils.Icon(FontAwesomeIcon.Times, Colors.Red);
                        ImGui.SameLine();
                        TextService.Draw("ListTab.Leve.Tooltip.Status.Incomplete");
                    }
                }

                if (isTownLocked)
                {
                    ImGuiUtils.Icon(FontAwesomeIcon.Exclamation, Colors.Yellow);
                    TextService.Draw("ListTab.Leve.Tooltip.TownLocked", item.Town.Value?.Name.ExtractText() ?? string.Empty);
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
                    AgentQuestJournal.Instance()->OpenForQuest(item.RowId, 2);
                    ImGui.SetWindowFocus(null);
                }
            }

            if (ImGui.BeginPopupContextItem($"##LeveContextMenu_{item.RowId}_Tooltip"))
            {
                var showSeparator = false;

                if (isAccepted)
                {
                    if (ImGui.Selectable(TextService.Translate("ListTab.Leve.ContextMenu.OpenInJournal")))
                    {
                        unsafe
                        {
                            AgentQuestJournal.Instance()->OpenForQuest(item.RowId, 2);
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

            if (LeveRequiredItemsCache.TryGetValue(item.RowId, out var requiredItems))
            {
                foreach (var entry in requiredItems)
                {
                    if (entry.Item is LeveHelperItem reqItem)
                    {
                        WindowState.DrawItem(reqItem, entry.Amount, $"##LeveTooltip_{item.RowId}_RequiredItems_{reqItem.RowId}");
                    }
                }
            }

            // Issuer
            ImGui.TableNextColumn();
            if (LeveIssuerCache.TryGetValue(item.RowId, out var issuers))
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

                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        MapService.OpenMap(ExcelService.FindRow<Level>(row => row?.Object == issuer.RowId));
                    }

                    if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        FilterManager.SetValue<LevemeteFilter>(issuer.RowId);
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
