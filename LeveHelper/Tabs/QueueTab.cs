using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCtor;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using HaselCommon.Services;
using LeveHelper.Config;
using LeveHelper.Services;

namespace LeveHelper.Tabs;

[RegisterSingleton, AutoConstruct]
public partial class QueueTab
{
    private readonly CraftQueueState _state;
    private readonly TextService _textService;
    private readonly LeveService _leveService;
    private readonly ExtendedItemService _itemService;
    private readonly PluginConfig _config;

    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem(_textService.Translate("Tabs.Queue"));
        if (!tab) return;

        window.RespectCloseHotkey = false;

        using var windowId = ImRaii.PushId("##QueueTab");

        if (!_leveService.HasAcceptedLeveQuests())
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(_textService.Translate("QueueTab.NoActiveLevequests"));
            return;
        }

        if (_config.ShowImportOnTeamCraftButton)
            DrawExportButton();

        var i = 0;

        if (_state.RequiredItems.Length != 0)
        {
            if (_state.Crystals.Length != 0)
            {
                ImGui.TextUnformatted(_textService.Translate("QueueTab.Category.Crystals"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in _state.Crystals)
                {
                    _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (_state.Gatherable.Length != 0)
            {
                ImGui.TextUnformatted(_textService.Translate("QueueTab.Category.Gather"));
                using var indent = ImRaii.PushIndent();
                foreach (var kv in _state.Gatherable)
                {
                    ImGui.TextUnformatted(_textService.GetPlaceName(kv.TerritoryType.PlaceName.RowId));

                    using var territoryIndent = ImRaii.PushIndent();
                    foreach (var entry in kv.Items)
                    {
                        _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true, kv.TerritoryType);
                    }
                }
            }

            if (_state.OtherSources.Length != 0)
            {
                ImGui.TextUnformatted(_textService.Translate("QueueTab.Category.Other"));
                using var indent = ImRaii.PushIndent();
                foreach (var entry in _state.OtherSources)
                {
                    _state.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (_state.Craftable.Length != 0)
            {
                ImGui.TextUnformatted(_textService.Translate("QueueTab.Category.Craft"));
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
            ImGui.TextUnformatted(_textService.Translate("QueueTab.ReadyForTurnIn"));
        }
    }

    private void DrawExportButton()
    {
        var needsToCraftItems = false;

        foreach (var leve in _leveService.GetActiveLeves())
        {
            var requiredItems = _leveService.GetRequiredItems(leve.RowId);
            if (requiredItems.Length == 0)
                continue;

            needsToCraftItems = true;
            break;
        }

        if (!needsToCraftItems)
            return;

        if (ImGui.Button(_textService.Translate("QueueTab.ImportOnTeamCraft")))
        {
            var items = new Dictionary<uint, (uint, uint)>(); // itemId = (recipeId, amount)

            foreach (var leve in _leveService.GetActiveLeves())
            {
                var requiredItems = _leveService.GetRequiredItems(leve.RowId);
                if (requiredItems.Length == 0)
                    continue;

                foreach (var item in requiredItems)
                {
                    if (items.TryGetValue(item.Item.RowId, out var tuple))
                    {
                        items[item.Item.RowId] = (tuple.Item1, tuple.Item2 + item.Amount);
                    }
                    else
                    {
                        var recipes = _itemService.GetRecipes(item.Item.RowId);

                        if (recipes != null && recipes.Count() == 1)
                            items[item.Item.RowId] = (recipes.First().RowId, item.Amount);
                        else
                            items[item.Item.RowId] = (0, item.Amount);
                    }
                }
            }

            var importstring = string.Join(';', items.Select(kv => $"{kv.Key},{(kv.Value.Item1 == 0 ? "null" : kv.Value.Item1)},{kv.Value.Item2}"));
            Util.OpenLink($"https://ffxivteamcraft.com/import/{Convert.ToBase64String(Encoding.UTF8.GetBytes(importstring))}");
        }
    }
}
