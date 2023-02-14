using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using static LeveHelper.ImGuiUtils;

namespace LeveHelper;

public class QueueTab
{
    public PluginWindow Window { get; }

    public QueueTab(PluginWindow window)
    {
        Window = window;
    }

    public void Draw()
    {
        if (Service.GameFunctions.ActiveLevequestsIds.Length == 0)
        {
            ImGui.TextDisabled("No active Levequests"); // center?
            return;
        }

        if (ImGuiComponents.IconButton(FontAwesomeIcon.RedoAlt))
        {
            Window.UpdateList();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.SetTooltip("Refresh");
        }

        var i = 0;

        if (Window.RequiredItems.Any())
        {
            if (Window.Crystals.Any())
            {
                ImGui.Text("Crystals:");
                foreach (var entry in Window.Crystals)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}", true);
                }
            }

            if (Window.Gatherable.Any())
            {
                ImGui.Text("Gather:");
                foreach (var kv in Window.Gatherable)
                {
                    ImGui.Text(kv.Item1.PlaceName);

                    foreach (var entry in kv.Item2)
                    {
                        DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}", true, kv.Item1);
                    }
                }
            }

            if (Window.OtherSources.Any())
            {
                ImGui.Text("Other:");
                foreach (var entry in Window.OtherSources)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}", true);
                }
            }

            if (Window.Craftable.Any())
            {
                ImGui.Text("Craft:");
                foreach (var entry in Window.Craftable)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}", true);
                }
            }
        }
    }
}
