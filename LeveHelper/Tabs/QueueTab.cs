using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using HaselCommon.Extensions;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Records;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public class QueueTab(WindowState WindowState, TextService TextService, ExcelService ExcelService, LeveService LeveService)
{
    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem(TextService.Translate("Tabs.Queue"));
        if (!tab) return;

        window.RespectCloseHotkey = false;

        using var windowId = ImRaii.PushId("##QueueTab");

        if (!LeveService.HasAcceptedLeveQuests())
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(TextService.Translate("QueueTab.NoActiveLevequests"));
            return;
        }

        var i = 0;

        if (WindowState.RequiredItems.Length != 0)
        {
            if (WindowState.Crystals.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Crystals");
                using var indent = ImRaii.PushIndent();
                foreach (var entry in WindowState.Crystals)
                {
                    WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (WindowState.Gatherable.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Gather");
                using var indent = ImRaii.PushIndent();
                foreach (var kv in WindowState.Gatherable)
                {
                    ImGui.TextUnformatted(ExcelService.GetRow<PlaceName>(kv.TerritoryType.PlaceName.Row)?.Name.ExtractText());

                    using var territoryIndent = ImRaii.PushIndent();
                    foreach (var entry in kv.Items)
                    {
                        WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true, kv.TerritoryType);
                    }
                }
            }

            if (WindowState.OtherSources.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Other");
                using var indent = ImRaii.PushIndent();
                foreach (var entry in WindowState.OtherSources)
                {
                    WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (WindowState.Craftable.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Craft");
                using var indent = ImRaii.PushIndent();
                foreach (var entry in WindowState.Craftable)
                {
                    // TODO: somehow show that the item is one of LeveRequiredItems, so we can craft it in HQ
                    // TODO: sort by dependency and job???
                    WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            // TODO: turn in queue?
        }
        else
        {
            TextService.Draw("QueueTab.ReadyForTurnIn");
        }
    }
}
