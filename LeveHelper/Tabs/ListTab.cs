using System.Globalization;
using System.Linq;
using System.Text;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Services;
using LeveHelper.Tables;

namespace LeveHelper.Tabs;

[RegisterSingleton]
public class ListTab(TextService TextService, LeveListTable LeveListTable, LeveService LeveService)
{
    private const int TextWrapBreakpoint = 820;

    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem(TextService.Translate("Tabs.Levequest"));
        if (!tab) return;

        window.RespectCloseHotkey = true;

        DrawInfoBar();
        LeveListTable.Draw();
    }

    private void DrawInfoBar()
    {
        using var windowId = ImRaii.PushId("##ListTab");

        var numLeveAllowances = LeveService.GetNumLeveAllowances();
        var numNeededAllowances = LeveListTable.GetNeededAllowances();
        var numCompletedLeves = GetNumCompletedLeves();
        var numTotalLeves = GetNumTotalLeves();

        var sb = new StringBuilder();

        sb.Append(TextService.Translate("ListTab.AcceptedLeves", LeveService.GetNumAcceptedLeveQuests()));
        sb.Append(ImGui.GetWindowSize().X > TextWrapBreakpoint ? " • " : "\n");

        var missing = numNeededAllowances - numLeveAllowances;
        var missingText = missing > 0
            ? TextService.Translate("ListTab.MissingText", missing, Math.Floor(missing / 6f))
            : string.Empty;

        sb.Append(TextService.Translate(
            "ListTab.Allowances",
            numLeveAllowances,
            numNeededAllowances,
            missingText,
            (QuestManager.GetNextLeveAllowancesDateTime() - DateTime.Now).ToString("hh':'mm':'ss")));

        if (numTotalLeves > 0)
        {
            sb.Append(ImGui.GetWindowSize().X > TextWrapBreakpoint ? " • " : "\n");

            var percent = (numCompletedLeves / (float)numTotalLeves * 100f).ToString("0.00", CultureInfo.InvariantCulture);
            sb.Append(TextService.Translate("ListTab.Completion", numCompletedLeves, numTotalLeves, percent));
        }

        ImGuiHelpers.SafeTextWrapped(sb.ToString());

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
        ImGui.Separator();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetStyle().FramePadding.Y);
    }

    private unsafe int GetNumTotalLeves()
    {
        var town = PlayerState.Instance()->StartTown;
        return LeveListTable.Rows.Count(leve => !LeveService.IsTownLocked(leve) || leve.Town.RowId == town);
    }

    private int GetNumCompletedLeves()
    {
        return LeveListTable.Rows.Count(leve => LeveService.IsComplete(leve));
    }
}
