using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Utils;
using ImGuiNET;
using LeveHelper.Sheets;
using LeveHelper.Utils;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;

namespace LeveHelper.Records;

public record WindowState
{
    public RequiredItem[] LeveRequiredItems { get; private set; } = Array.Empty<RequiredItem>();
    public QueuedItem[] RequiredItems { get; private set; } = Array.Empty<QueuedItem>();
    public QueuedItem[] Crystals { get; private set; } = Array.Empty<QueuedItem>();
    public ZoneItems[] Gatherable { get; private set; } = Array.Empty<ZoneItems>();
    public QueuedItem[] OtherSources { get; private set; } = Array.Empty<QueuedItem>();
    public QueuedItem[] Craftable { get; private set; } = Array.Empty<QueuedItem>();

    public void UpdateList()
    {
        var acceptedCraftAndGatherLeves = QuestUtils.GetActiveLeves()
            .Where(leve => leve.RequiredItems.Length > 0);

        // list all items required for CraftLeves and GatherLeves
        LeveRequiredItems = acceptedCraftAndGatherLeves
            .SelectMany(leve => leve.RequiredItems)
            .ToArray();

        // gather a list of all items needed to craft everything
        var neededAmounts = new Dictionary<uint, QueuedItem>();
        foreach (var leve in acceptedCraftAndGatherLeves)
        {
            foreach (var entry in leve.RequiredItems)
            {
                if (entry.Item is Item item)
                    TraverseItems(item, entry.Amount, neededAmounts);
            }
        }

        RequiredItems = neededAmounts
            .Where(kv => kv.Value.AmountLeft > 0) // filter every completed item
            .Select(kv => kv.Value)
            .ToArray();

        // categorize
        Crystals = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.Crystals)
            .ToArray();

        // special case for Gatherables
        {
            var gatherables = RequiredItems
                .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.Gatherable)
                .ToArray();

            // group items by zone
            var zones = new Dictionary<uint, HashSet<QueuedItem>>();
            foreach (var entry in gatherables)
            {
                foreach (var point in entry.Item.GatheringPoints)
                {
                    if (zones.TryGetValue(point.TerritoryType.Row, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(point.TerritoryType.Row, new() { entry });
                    }
                }

                foreach (var spot in entry.Item.FishingSpots)
                {
                    if (zones.TryGetValue(spot.TerritoryType.Row, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(spot.TerritoryType.Row, new() { entry });
                    }
                }
            }

            // for each item, get the one zone with the most items in it
            var groupedGatherables = new Dictionary<uint, ZoneItems>();
            foreach (var entry in gatherables)
            {
                var zone = zones
                    .Where(zone => zone.Value.Select(e => e.Item.RowId).Contains(entry.Item.RowId))
                    .OrderByDescending(zone => zone.Value.Count)
                    .ThenBy(zone => zone.Key.CompareTo(Service.ClientState.TerritoryType))
                    .First();

                if (!groupedGatherables.ContainsKey(zone.Key))
                    groupedGatherables.Add(zone.Key, new(GetRow<TerritoryType>(zone.Key)!, zone.Value));
            }

            // sorting by cheapest teleport costs
            var zoneItems = groupedGatherables.Values.ToList();
            zoneItems.Insert(0, new(GetRow<TerritoryType>(Service.ClientState.TerritoryType)!, new())); // add starting zone

            var nodes = new Dictionary<(ZoneItems, ZoneItems), uint>();
            foreach (var zoneItemFrom in zoneItems)
            {
                foreach (var zoneItemTo in zoneItems)
                {
                    var cost = (uint)Telepo.GetTeleportCost((ushort)zoneItemFrom.TerritoryType.RowId, (ushort)zoneItemTo.TerritoryType.RowId, false, false, false);
                    nodes.Add((zoneItemFrom, zoneItemTo), cost);
                }
            }

            Gatherable = nodes
                .Where(kv => kv.Key.Item2.Items.Count != 0) // filter starting zone
                .OrderBy(kv => kv.Value) // sort by cost
                .Select(kv => kv.Key.Item2)
                .Distinct()
                .ToArray();
        }

        OtherSources = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.OtherSources)
            .ToArray();

        Craftable = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.Craftable)
            .ToArray();
    }

    private static void TraverseItems(Item item, uint amount, Dictionary<uint, QueuedItem> neededAmounts)
    {
        var newNode = false;
        if (!neededAmounts.TryGetValue(item.RowId, out var node))
        {
            item.UpdateQuantityOwned();
            node = new(item, 0);
            newNode = true;
        }

        node.AmountNeeded += amount;

        if (node.AmountLeft != 0)
        {
            foreach (var dependency in item.Ingredients)
            {
                if (dependency.Item is Item dependencyItem)
                {
                    float resultAmount = dependencyItem.IsCraftable ? dependencyItem.Recipes.First().AmountResult : 1;
                    var totalAmount = (uint)Math.Ceiling((double)amount * dependency.Amount / resultAmount);
                    TraverseItems(dependencyItem, totalAmount, neededAmounts);
                }
            }
        }

        if (newNode)
            neededAmounts.Add(item.RowId, node);
    }

    public void DrawIngredients(string key, RequiredItem[] ingredients, uint parentAmount = 1, int depth = 0)
    {
        if (depth > 0)
            ImGui.Indent();

        foreach (var entry in ingredients)
        {
            var ingredient = entry.Item;
            var ingredientAmount = entry.Amount * parentAmount;

            if (ingredient is Item ingredientItem)
            {
                // filter crystals completely if we have enough
                if (ingredientItem.IsCrystal && ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                DrawItem(ingredientItem, ingredientAmount, $"{key}_{ingredient.RowId}");

                // filter ingredients if we have enough
                if (ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                if (ingredientItem.Ingredients.Any())
                {
                    float resultAmount = ingredientItem.IsCraftable ? ingredientItem.Recipes.First().AmountResult : 1;
                    var ingredientCount = (uint)(ingredientAmount / resultAmount);
                    DrawIngredients($"{key}_{ingredient.RowId}", ingredientItem.Ingredients, ingredientCount, depth + 1);
                }
            }
        }

        if (depth > 0)
            ImGui.Unindent();
    }

    public void DrawItem(Item? item, uint neededCount = 0, string key = "Item", bool showIndicators = false, TerritoryType? territoryType = null)
    {
        if (item == null)
            return;

        if (key == "Item")
            key += "_" + item.RowId.ToString();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = LeveRequiredItems.Any(entry => entry.Item.RowId == item.RowId);

        Service.TextureManager.GetIcon(item.Icon, isLeveRequiredItem).Draw(20);
        ImGui.SameLine();

        var color = Colors.White;

        if (item.QuantityOwned >= neededCount)
            color = Colors.Green;
        else if (item.QuantityOwned < neededCount || item.HasAllIngredients == false)
            color = Colors.Grey;

        ImGui.PushStyleColor(ImGuiCol.Text, (uint)color);
        ImGui.Selectable($"{(neededCount > 0 ? $"{item.QuantityOwned}/{neededCount} " : "")}{item.Name}{(isLeveRequiredItem ? (char)SeIconChar.HighQuality : "")}##{key}_Selectable");
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            GetItemName(item.RowId); // cache item name

            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (item.IsCraftable == true)
            {
                ImGui.SetTooltip(GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (item.IsGatherable == true)
            {
                if (territoryType != null)
                {
                    ImGui.SetTooltip(GetAddonText(8506)); // "Open Map"
                }
                else
                {
                    ImGui.SetTooltip(GetAddonText(1472)); // "Search for Item by Gathering Method"
                }
            }
            else if (item.IsFish == true)
            {
                ImGui.SetTooltip(GetAddonText(8506)); // "Open Map"
            }
            else
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(t("ItemContextMenu.OpenOnGarlandTools"));

                var pos = ImGui.GetCursorPos();
                ImGui.GetWindowDrawList().AddText(
                    UiBuilder.IconFont, 12,
                    ImGui.GetWindowPos() + pos + new Vector2(2),
                    Colors.Grey,
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(Colors.Grey, $"https://www.garlandtools.org/db/#item/{item.RowId}");
                ImGui.EndTooltip();
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (item.IsCraftable == true)
            {
                unsafe
                {
                    AgentRecipeNote.Instance()->OpenRecipeByItemId(item.RowId);
                    ImGui.SetWindowFocus(null);
                }
            }
            // TODO: preferance setting?
            else if (item.IsGatherable == true)
            {
                unsafe
                {
                    if (territoryType != null)
                    {
                        var point = item.GatheringPoints.First(point => point.TerritoryType.Row == territoryType.RowId);
                        point.OpenMap(item, "LeveHelper");
                    }
                    else
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                    }

                    ImGui.SetWindowFocus(null);
                }
            }
            else if (item.IsFish == true)
            {
                item.FishingSpots.First().OpenMap(item, "LeveHelper");
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
        }

        new ImGuiContextMenu($"##ItemContextMenu_{key}_Tooltip")
        {
            ImGuiContextMenu.CreateSearchCraftingMethod(item),
            ImGuiContextMenu.CreateSearchGatheringMethod(item),
            ImGuiContextMenu.CreateOpenInFishGuide(item),
            ImGuiContextMenu.CreateOpenMapForGatheringPoint(item, territoryType, "LeveHelper"),
            ImGuiContextMenu.CreateOpenMapForFishingSpot(item, "LeveHelper"),
            ImGuiContextMenu.CreateSeparator(),
            ImGuiContextMenu.CreateItemFinder(item.RowId),
            ImGuiContextMenu.CreateCopyItemName(item.RowId),
            ImGuiContextMenu.CreateItemSearch(item),
            ImGuiContextMenu.CreateOpenOnGarlandTools(item.RowId),
        }
        .Draw();

        var classJobIcon = 0u;

        if (item.IsCraftable)
        {
            classJobIcon = 62008 + item.Recipes.First().CraftType.Row;
        }
        else if (item.IsGatherable)
        {
            classJobIcon = item.GatheringPoints.First().Icon;
        }
        else if (item.IsFish)
        {
            classJobIcon = item.FishingSpots.First().Icon;
        }

        if (showIndicators && classJobIcon != 0)
        {
            var availSize = ImGui.GetContentRegionMax();

            if (item.IsGatherable && !item.GatheringItems.Any(gi => !gi.IsHidden))
            {
                var text = GetAddonText(627); // or 629? "Hidden"
                var textWidth = ImGui.CalcTextSize(text).X;
                ImGui.SameLine(availSize.X - textWidth - ImGui.GetStyle().ItemInnerSpacing.X - 20, 0);
                ImGui.TextUnformatted(text);
            }

            ImGui.SameLine(availSize.X - 20, 0);
            Service.TextureManager.GetIcon(classJobIcon).Draw(20);
        }
    }
}
