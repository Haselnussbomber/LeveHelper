using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Services;
using HaselCommon.Utils;
using HaselCommon.Utils.Globals;
using ImGuiNET;
using LeveHelper.Caches;
using LeveHelper.Sheets;
using Lumina.Text;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;

namespace LeveHelper.Records;

public record WindowState(
    IClientState ClientState,
    ExcelService ExcelService,
    TextureService TextureService,
    TextService TextService,
    MapService MapService,
    TeleportService TeleportService,
    ImGuiContextMenuService ImGuiContextMenuService,
    LeveService QuestService,
    LeveRequiredItemsCache LeveRequiredItemsCache)
{
    public RequiredItem[] LeveRequiredItems { get; private set; } = [];
    public QueuedItem[] RequiredItems { get; private set; } = [];
    public QueuedItem[] Crystals { get; private set; } = [];
    public ZoneItems[] Gatherable { get; private set; } = [];
    public QueuedItem[] OtherSources { get; private set; } = [];
    public QueuedItem[] Craftable { get; private set; } = [];

    public void UpdateList()
    {
        var acceptedCraftAndGatherLeves = QuestService.GetActiveLeves()
            .Where(leve => LeveRequiredItemsCache.TryGetValue(leve.RowId, out var requiredItems) && requiredItems.Length > 0);

        // list all items required for CraftLeves and GatherLeves
        LeveRequiredItems = acceptedCraftAndGatherLeves
            .SelectMany(leve => LeveRequiredItemsCache.GetValue(leve.RowId) ?? [])
            .ToArray();

        // gather a list of all items needed to craft everything
        var neededAmounts = new Dictionary<uint, QueuedItem>();
        foreach (var leve in acceptedCraftAndGatherLeves)
        {
            foreach (var entry in LeveRequiredItemsCache.GetValue(leve.RowId) ?? [])
            {
                if (entry.Item is LeveHelperItem item)
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
                    .ThenBy(zone => zone.Key.CompareTo(ClientState.TerritoryType))
                    .First();

                if (!groupedGatherables.ContainsKey(zone.Key))
                    groupedGatherables.Add(zone.Key, new(ExcelService.GetRow<TerritoryType>(zone.Key)!, zone.Value));
            }

            Gatherable = groupedGatherables.Values.Distinct().ToArray();
        }

        OtherSources = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.OtherSources)
            .ToArray();

        Craftable = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.Craftable)
            .ToArray();
    }

    private static void TraverseItems(LeveHelperItem item, uint amount, Dictionary<uint, QueuedItem> neededAmounts)
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
                if (dependency.Item is LeveHelperItem dependencyItem)
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

            if (ingredient is LeveHelperItem ingredientItem)
            {
                // filter crystals completely if we have enough
                if (ingredientItem.IsCrystal && ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                DrawItem(ingredientItem, ingredientAmount, $"{key}_{ingredient.RowId}");

                // filter ingredients if we have enough
                if (ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                if (ingredientItem.Ingredients.Length != 0)
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

    public void DrawItem(LeveHelperItem? item, uint neededCount = 0, string key = "Item", bool showIndicators = false, TerritoryType? territoryType = null)
    {
        if (item == null)
            return;

        if (key == "Item")
            key += "_" + item.RowId.ToString();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = LeveRequiredItems.Any(entry => entry.Item.RowId == item.RowId);

        TextureService.DrawIcon(new GameIconLookup(item.Icon, isLeveRequiredItem), 20);
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
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (item.IsCraftable == true)
            {
                ImGui.SetTooltip(TextService.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (item.IsGatherable == true)
            {
                if (territoryType != null)
                {
                    ImGui.SetTooltip(TextService.GetAddonText(8506)); // "Open Map"
                }
                else
                {
                    ImGui.SetTooltip(TextService.GetAddonText(1472)); // "Search for Item by Gathering Method"
                }
            }
            else if (item.IsFish == true)
            {
                ImGui.SetTooltip(TextService.GetAddonText(8506)); // "Open Map"
            }
            else
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();
                TextService.Draw("ItemContextMenu.OpenOnGarlandTools");

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
                        MapService.OpenMap(point, item, "LeveHelper");
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
                item.FishingSpots.First().OpenMap(item, new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString());
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
        }

        ImGuiContextMenuService.Draw($"##ItemContextMenu_{key}_Tooltip", (builder) =>
        {
            builder
                .AddSearchCraftingMethod(item)
                .AddSearchGatheringMethod(item)
                .AddOpenInFishGuide(item)
                .AddOpenMapForGatheringPoint(item, territoryType, new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString())
                .AddOpenMapForFishingSpot(item, new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString())
                .AddSeparator()
                .AddItemFinder(item.RowId)
                .AddCopyItemName(item.RowId)
                .AddItemSearch(item)
                .AddOpenOnGarlandTools("item", item.RowId);
        });

        var classJobIcon = 0u;

        if (item.IsCraftable)
        {
            classJobIcon = 62008 + item.Recipes.First().CraftType.Row;
        }
        else if (item.IsGatherable)
        {
            var point = item.GatheringPoints.First();
            var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
            var rare = !Statics.IsGatheringTypeRare(point.Type);
            classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
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
                var text = TextService.GetAddonText(627); // or 629? "Hidden"
                var textWidth = ImGui.CalcTextSize(text).X;
                ImGui.SameLine(availSize.X - textWidth - ImGui.GetStyle().ItemInnerSpacing.X - 20, 0);
                ImGui.TextUnformatted(text);
            }

            ImGui.SameLine(availSize.X - 20, 0);
            TextureService.DrawIcon(classJobIcon, 20);
        }
    }
}
