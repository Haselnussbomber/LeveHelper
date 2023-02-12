using System.Globalization;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace LeveHelper;

public class VendorListWindow : Window
{
    public CachedItem? Item { get; internal set; }

    public VendorListWindow() : base("LeveHelper - Vendor List")
    {
        base.Size = new Vector2(320, 400);
        base.SizeCondition = ImGuiCond.Appearing;
        base.SizeConstraints = new()
        {
            MinimumSize = new Vector2(320, 400),
            MaximumSize = new Vector2(4096, 2160)
        };
    }

    private static float toCoord(float value, ushort scale)
    {
        var tileScale = 2048f / 41f;
        return value / tileScale + 2048f / (scale / 100f) / tileScale / 2 + 1;
    }

    public override void Draw()
    {
        if (Item == null)
            return;

        ImGuiUtils.DrawIcon(Item.Icon, 20, 20);
        ImGui.SameLine();
        ImGui.Text($"{Item.ItemName}");
        ImGui.Separator();
        ImGui.Text("Sold by:");

        foreach (var vendorId in Item.Vendors)
        {
            var npc = NpcCache.Get(vendorId);
            if (npc == null || npc.Level == null || npc.Level.Map.Value == null || npc.Level.Map.Value.PlaceName.Value == null)
            {
                continue;
            }

            var map = npc.Level.Map.Value;
            var x = toCoord(npc.Level.X, map.SizeFactor) + map.OffsetX;
            var y = toCoord(npc.Level.Z, map.SizeFactor) + map.OffsetY;

            ImGui.Selectable($"{npc.Name}, {map.PlaceName.Value!.Name} ({x.ToString("0.0", CultureInfo.InvariantCulture)}, {y.ToString("0.0", CultureInfo.InvariantCulture)})");

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Open Map");
            }

            if (ImGui.IsItemClicked())
            {
                GameFunctions.OpenMapWithMapLink(npc.Level);
            }
        }
    }
}
