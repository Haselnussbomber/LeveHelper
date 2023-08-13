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
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(t("QueueTab.NoActiveLevequests"));
            return;
        }

        if (ImGuiUtils.IconButton("##Refresh", FontAwesomeIcon.RedoAlt, t("QueueTab.Refresh")))
        {
            Window.UpdateList();
        }

        var i = 0;

        if (Window.RequiredItems.Any())
        {
            if (Window.Crystals.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Crystals"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in Window.Crystals)
                {
                    Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (Window.Gatherable.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Gather"));
                using var indent = ImRaii.PushIndent();
                foreach (var kv in Window.Gatherable)
                {
                    ImGui.TextUnformatted(kv.TerritoryType.PlaceName);

                    using var territoryIndent = ImRaii.PushIndent();
                    foreach (var entry in kv.Items)
                    {
                        Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true, kv.TerritoryType);
                    }
                }
            }

            if (Window.OtherSources.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Other"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in Window.OtherSources)
                {
                    Window.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (Window.Craftable.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Craft"));
                using var indent = ImRaii.PushIndent();
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
            ImGui.TextUnformatted(t("QueueTab.ReadyForTurnIn"));
        }
    }
}
