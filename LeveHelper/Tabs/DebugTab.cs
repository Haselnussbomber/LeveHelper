using System.Diagnostics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Records;
using LeveHelper.Sheets;
using GatheringItem = HaselCommon.Sheets.ExtendedGatheringItem;

namespace LeveHelper;

public class DebugTab(WindowState WindowState, ExcelService ExcelService, MapService MapService)
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

        var itemSheet = ExcelService.GetSheet<LeveHelperItem>();
        foreach (var item in ExcelService.GetSheet<GatheringItem>())
        {
            if (item.RowId == 0 || item.Item == 0 || item.Item >= 1000000)
                continue;

            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(item.RowId.ToString());

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(item.Item.ToString());
            ImGui.SameLine();
            WindowState.DrawItem(ExcelService.GetRow<LeveHelperItem>(item.ItemRow.Row));

            ImGui.TableNextColumn();
            foreach (var point in item.GatheringPoints)
            {
                ImGui.TextUnformatted($"{point.RowId} => {point.PlaceName.Value?.Name ?? ""}");
                if (ImGui.IsItemHovered())
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                if (ImGui.IsItemClicked())
                    MapService.OpenMap(point, item.ItemRow.Value, "LeveHelper");
            }
        }
    }
}
