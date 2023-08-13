using Dalamud.Interface.Raii;
using ImGuiNET;
using LeveHelper.Sheets;

namespace LeveHelper;

public class DebugTab
{
    public PluginWindow Window { get; }

    public DebugTab(PluginWindow window)
    {
        Window = window;
    }

    public void Draw()
    {
        using var windowId = ImRaii.PushId("##DebugTab");
        using var tabbar = ImRaii.TabBar("##DebugTabBar");

        using (var tabItem = ImRaii.TabItem("Gathering Items"))
        {
            if (tabItem.Success)
            {
                using var table = ImRaii.Table("##GatheringItemsTable", 3);
                ImGui.TableSetupColumn("RowId", ImGuiTableColumnFlags.WidthFixed, 50);
                ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("GatheringPoints", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableHeadersRow();

                var itemSheet = GetSheet<Item>();
                foreach (var item in GetSheet<GatheringItem>())
                {
                    if (item.RowId == 0 || item.Item == 0 || item.Item >= 1000000)
                        continue;

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted(item.RowId.ToString());

                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted(item.Item.ToString());
                    ImGui.SameLine();
                    Window.DrawItem(item.ItemRow.Value);

                    ImGui.TableNextColumn();
                    foreach (var point in item.GatheringPoints)
                    {
                        ImGui.TextUnformatted($"{point.RowId} => {point.PlaceName.Value?.Name ?? ""}");
                        if (ImGui.IsItemHovered())
                            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        if (ImGui.IsItemClicked())
                            Service.GameFunctions.OpenMapWithGatheringPoint(point, item.ItemRow.Value);
                    }
                }
            }
        }
    }
}
