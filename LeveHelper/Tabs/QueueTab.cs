using System.Linq;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ImGuiNET;
using LeveHelper.Records;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public class QueueTab
{
    private readonly WindowState _state;

    public QueueTab(WindowState state)
    {
        _state = state;
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

        var i = 0;

        if (_state.RequiredItems.Any())
        {
            if (_state.Crystals.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Crystals"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in _state.Crystals)
                {
                    _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (_state.Gatherable.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Gather"));
                using var indent = ImRaii.PushIndent();
                foreach (var kv in _state.Gatherable)
                {
                    ImGui.TextUnformatted(GetRow<PlaceName>(kv.TerritoryType.PlaceName.Row)?.Name.ToDalamudString().ToString());

                    using var territoryIndent = ImRaii.PushIndent();
                    foreach (var entry in kv.Items)
                    {
                        _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true, kv.TerritoryType);
                    }
                }
            }

            if (_state.OtherSources.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Other"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in _state.OtherSources)
                {
                    _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (_state.Craftable.Any())
            {
                ImGui.TextUnformatted(t("QueueTab.Category.Craft"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in _state.Craftable)
                {
                    // TODO: somehow show that the item is one of LeveRequiredItems, so we can craft it in HQ
                    // TODO: sort by dependency and job???
                    _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
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
