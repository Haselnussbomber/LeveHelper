using System.Diagnostics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Records;
using LeveHelper.Services;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public class DebugTab(WindowState WindowState, ExcelService ExcelService, MapService MapService, ExtendedItemService ItemService)
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

        using var table = ImRaii.Table("##GatheringItemsTable", 3);
        ImGui.TableSetupColumn("RowId", ImGuiTableColumnFlags.WidthFixed, 50);
        ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("GatheringPoints", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableHeadersRow();

        var itemSheet = ExcelService.GetSheet<Item>();
        foreach (var gatheringItem in ExcelService.GetSheet<GatheringItem>())
        {
            if (gatheringItem.RowId == 0 || gatheringItem.Item == 0 || gatheringItem.Item >= 1000000)
                continue;

            var item = ExcelService.GetRow<Item>((uint)gatheringItem.Item);
            if (item == null)
                continue;

            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(gatheringItem.RowId.ToString());

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(gatheringItem.Item.ToString());
            ImGui.SameLine();
            WindowState.DrawItem(item);

            ImGui.TableNextColumn();
            foreach (var point in ItemService.GetGatheringPoints(gatheringItem))
            {
                ImGui.TextUnformatted($"{point.RowId} => {point.PlaceName.Value?.Name ?? ""}");
                if (ImGui.IsItemHovered())
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                if (ImGui.IsItemClicked())
                    MapService.OpenMap(point, item, "LeveHelper");
            }
        }
    }
}
