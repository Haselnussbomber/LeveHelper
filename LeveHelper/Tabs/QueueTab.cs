using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Config;
using LeveHelper.Records;
using LeveHelper.Services;

namespace LeveHelper;

[RegisterSingleton]
public class QueueTab(
    WindowState WindowState,
    TextService TextService,
    LeveService LeveService,
    ExtendedItemService ItemService,
    PluginConfig PluginConfig)
{
    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem(TextService.Translate("Tabs.Queue"));
        if (!tab) return;

        window.RespectCloseHotkey = false;

        using var windowId = ImRaii.PushId("##QueueTab");

        if (!LeveService.HasAcceptedLeveQuests())
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(TextService.Translate("QueueTab.NoActiveLevequests"));
            return;
        }

        if (PluginConfig.ShowImportOnTeamCraftButton)
            DrawExportButton();

        var i = 0;

        if (WindowState.RequiredItems.Length != 0)
        {
            if (WindowState.Crystals.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Crystals");
                using var indent = ImRaii.PushIndent();
                foreach (var entry in WindowState.Crystals)
                {
                    WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (WindowState.Gatherable.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Gather");
                using var indent = ImRaii.PushIndent();
                foreach (var kv in WindowState.Gatherable)
                {
                    ImGui.TextUnformatted(kv.TerritoryType.PlaceName.Value.Name.ExtractText());

                    using var territoryIndent = ImRaii.PushIndent();
                    foreach (var entry in kv.Items)
                    {
                        WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true, kv.TerritoryType);
                    }
                }
            }

            if (WindowState.OtherSources.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Other");
                using var indent = ImRaii.PushIndent();
                foreach (var entry in WindowState.OtherSources)
                {
                    WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            if (WindowState.Craftable.Length != 0)
            {
                TextService.Draw("QueueTab.Category.Craft");
                using var indent = ImRaii.PushIndent();
                foreach (var entry in WindowState.Craftable)
                {
                    // TODO: somehow show that the item is one of LeveRequiredItems, so we can craft it in HQ
                    // TODO: sort by dependency and job???
                    WindowState.DrawItem(entry.Item, entry.AmountNeeded, $"Item{i++}", true);
                }
            }

            // TODO: turn in queue?
        }
        else
        {
            TextService.Draw("QueueTab.ReadyForTurnIn");
        }
    }

    private void DrawExportButton()
    {
        var needsToCraftItems = false;

        foreach (var leve in LeveService.GetActiveLeves())
        {
            var requiredItems = LeveService.GetRequiredItems(leve);
            if (requiredItems.Length == 0)
                continue;

            needsToCraftItems = true;
            break;
        }

        if (!needsToCraftItems)
            return;

        if (ImGui.Button(TextService.Translate("QueueTab.ImportOnTeamCraft")))
        {
            var items = new Dictionary<uint, (uint, uint)>(); // itemId = (recipeId, amount)

            foreach (var leve in LeveService.GetActiveLeves())
            {
                var requiredItems = LeveService.GetRequiredItems(leve);
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
                        var recipes = ItemService.GetRecipes(item.Item);

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
