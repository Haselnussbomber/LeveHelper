using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Raii;
using ImGuiNET;
using LeveHelper.Utils;

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
        using var windowId = ImRaii.PushId("##QueueTab");

        if (Service.GameFunctions.ActiveLevequestsIds.Length == 0)
        {
            ImGui.TextDisabled("No active Levequests"); // center?
            return;
        }

        if (ImGuiUtils.IconButton("##Refresh", FontAwesomeIcon.RedoAlt, "Refresh"))
        {
            Window.UpdateList();
        }

        var i = 0;

        if (Window.RequiredItems.Any())
        {
            if (Window.Crystals.Any())
            {
                ImGui.Text("Crystals:");
                foreach (var entry in Window.Crystals)
                {
                    Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (Window.Gatherable.Any())
            {
                ImGui.Text("Gather:");
                foreach (var kv in Window.Gatherable)
                {
                    ImGui.Text(kv.TerritoryType.PlaceName);

                    foreach (var entry in kv.Items)
                    {
                        Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true, kv.TerritoryType);
                    }
                }
            }

            if (Window.OtherSources.Any())
            {
                ImGui.Text("Other:");
                foreach (var entry in Window.OtherSources)
                {
                    Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (Window.Craftable.Any())
            {
                ImGui.Text("Craft:");
                foreach (var entry in Window.Craftable)
                {
                    // TODO: somehow show that the item is one of LeveRequiredItems, so we can craft it in HQ
                    // TODO: sort by dependency and job???
                    Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            // TODO: turn in queue?
        }
        else
        {
            ImGui.Text("Ready for turn in!");
        }
    }
}
