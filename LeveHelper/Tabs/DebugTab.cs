using System.Diagnostics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using HaselCommon.Services;
using LeveHelper.Services;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tabs;

[RegisterSingleton]
public class DebugTab(
    CraftQueueState state,
    ExcelService excelService,
    MapService mapService,
    ExtendedItemService itemService,
    TextService textService)
{
    [Conditional("DEBUG")]
    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem("Debug");
        if (!tab) return;

        window.RespectCloseHotkey = true;

        using var windowId = ImRaii.PushId("##DebugTab");
        using var tabbar = ImRaii.TabBar("##DebugTabBar");

        using var tabItem = ImRaii.TabItem("Gathering Items");
        if (!tabItem) return;

        using var table = ImRaii.Table("##GatheringItemsTable", 3, ImGuiTableFlags.RowBg);
        ImGui.TableSetupColumn("RowId", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("GatheringPoints", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableHeadersRow();

        foreach (var gatheringItem in excelService.GetSheet<GatheringItem>())
        {
            if (gatheringItem.RowId == 0 || gatheringItem.Item.RowId == 0 || gatheringItem.Item.RowId >= 1000000)
                continue;

            if (!gatheringItem.Item.TryGetValue<Item>(out var item))
                continue;

            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(gatheringItem.RowId.ToString());

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(gatheringItem.Item.RowId.ToString());
            ImGui.SameLine();
            state.DrawItem(item);

            ImGui.TableNextColumn();

            using var node = ImRaii.TreeNode($"{itemService.GetGatheringPoints(gatheringItem).Length} Gathering Points###GatheringPoints_{gatheringItem.RowId}", ImGuiTreeNodeFlags.SpanAvailWidth);
            if (!node) continue;

            foreach (var point in itemService.GetGatheringPoints(gatheringItem))
            {
                ImGui.TextUnformatted($"{point.RowId} => {textService.GetPlaceName(point.PlaceName.RowId)}");
                if (ImGui.IsItemHovered())
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                if (ImGui.IsItemClicked())
                    mapService.OpenMap(point, item, "LeveHelper");
            }
        }
    }
}
