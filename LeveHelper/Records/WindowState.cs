using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Extensions.Sheets;
using HaselCommon.Game;
using HaselCommon.Graphics;
using HaselCommon.Services;
using HaselCommon.Utils;
using ImGuiNET;
using LeveHelper.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using Lumina.Text;
using TerritoryType = Lumina.Excel.Sheets.TerritoryType;

namespace LeveHelper.Records;

public record WindowState(
    IClientState ClientState,
    ExcelService ExcelService,
    TextureService TextureService,
    TextService TextService,
    MapService MapService,
    TeleportService TeleportService,
    ImGuiContextMenuService ImGuiContextMenuService,
    LeveService LeveService,
    ExtendedItemService ItemService)
{
    public ItemAmount[] LeveRequiredItems { get; private set; } = [];
    public QueuedItem[] RequiredItems { get; private set; } = [];
    public QueuedItem[] Crystals { get; private set; } = [];
    public ZoneItems[] Gatherable { get; private set; } = [];
    public QueuedItem[] OtherSources { get; private set; } = [];
    public QueuedItem[] Craftable { get; private set; } = [];

    public void UpdateList()
    {
        var acceptedCraftAndGatherLeves = LeveService.GetActiveLeves()
            .Select(leve => (Leve: leve, Items: LeveService.GetRequiredItems(leve)))
            .Where(items => items.Items.Length > 0);

        // list all items required for CraftLeves and GatherLeves
        LeveRequiredItems = acceptedCraftAndGatherLeves
            .SelectMany(entry => entry.Items)
            .ToArray();

        // gather a list of all items needed to craft everything
        var neededAmounts = new Dictionary<uint, QueuedItem>();
        foreach (var li in acceptedCraftAndGatherLeves)
        {
            foreach (var itemAmount in li.Items)
            {
                TraverseItems(itemAmount.Item, itemAmount.Amount, neededAmounts);
            }
        }

        RequiredItems = neededAmounts
            .Where(kv => kv.Value.AmountLeft > 0) // filter every completed item
            .Select(kv => kv.Value)
            .ToArray();

        // categorize
        Crystals = RequiredItems
            .Where(entry => ItemService.GetQueueCategory(entry.Item) == ItemQueueCategory.Crystals)
            .ToArray();

        // special case for Gatherables
        {
            var gatherables = RequiredItems
                .Where(entry => ItemService.GetQueueCategory(entry.Item) == ItemQueueCategory.Gatherable)
                .ToArray();

            // group items by zone
            var zones = new Dictionary<uint, HashSet<QueuedItem>>();
            foreach (var entry in gatherables)
            {
                foreach (var point in ItemService.GetGatheringPoints(entry.Item))
                {
                    if (zones.TryGetValue(point.TerritoryType.RowId, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(point.TerritoryType.RowId, [entry]);
                    }
                }

                foreach (var spot in ItemService.GetFishingSpots(entry.Item))
                {
                    if (zones.TryGetValue(spot.TerritoryType.RowId, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(spot.TerritoryType.RowId, [entry]);
                    }
                }

                foreach (var point in ItemService.GetSpearfishingGatheringPoints(entry.Item))
                {
                    if (zones.TryGetValue(point.TerritoryType.RowId, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(point.TerritoryType.RowId, [entry]);
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

                if (!groupedGatherables.ContainsKey(zone.Key) && ExcelService.TryGetRow<TerritoryType>(zone.Key, out var territoryType))
                    groupedGatherables.Add(zone.Key, new(territoryType.AsRef(), zone.Value));
            }

            Gatherable = groupedGatherables.Values.Distinct().ToArray();
        }

        OtherSources = RequiredItems
            .Where(entry => ItemService.GetQueueCategory(entry.Item) == ItemQueueCategory.OtherSources)
            .ToArray();

        Craftable = RequiredItems
            .Where(entry => ItemService.GetQueueCategory(entry.Item) == ItemQueueCategory.Craftable)
            .ToArray();
    }

    private void TraverseItems(Item item, uint amount, Dictionary<uint, QueuedItem> neededAmounts)
    {
        var newNode = false;
        if (!neededAmounts.TryGetValue(item.RowId, out var node))
        {
            ItemService.InvalidateQuantity(item);
            node = new(item, 0, ItemService);
            newNode = true;
        }

        node.AmountNeeded += amount;

        if (node.AmountLeft != 0)
        {
            float resultAmount = ItemService.IsCraftable(item) ? ItemService.GetRecipes(item).First().AmountResult : 1;

            foreach (var dependency in ItemService.GetIngredients(item))
            {
                var totalAmount = (uint)Math.Ceiling(amount / resultAmount) * dependency.Amount;
                TraverseItems(dependency.Item, totalAmount, neededAmounts);
            }
        }

        if (newNode)
            neededAmounts.Add(item.RowId, node);
    }

    public void DrawIngredients(string key, ItemAmount[] ingredients, uint parentAmount = 1, int depth = 0)
    {
        if (depth > 0)
            ImGui.Indent();

        foreach (var entry in ingredients)
        {
            var ingredientAmount = entry.Amount * parentAmount;

            // filter crystals completely if we have enough
            if (ItemService.IsCrystal(entry.Item) && ItemService.GetQuantity(entry.Item) >= ingredientAmount)
                continue;

            DrawItem(entry.Item, ingredientAmount, $"{key}_{entry.Item.RowId}");

            // filter ingredients if we have enough
            if (ItemService.GetQuantity(entry.Item) >= ingredientAmount)
                continue;

            var entryIngredients = ItemService.GetIngredients(entry.Item);
            if (entryIngredients.Length != 0)
            {
                float resultAmount = ItemService.IsCraftable(entry.Item) ? ItemService.GetRecipes(entry.Item).First().AmountResult : 1;
                var ingredientCount = (uint)Math.Ceiling(ingredientAmount / resultAmount);
                DrawIngredients($"{key}_{entry.Item.RowId}", entryIngredients, ingredientCount, depth + 1);
            }
        }

        if (depth > 0)
            ImGui.Unindent();
    }

    public void DrawItem(Item item, uint neededCount = 0, string key = "Item", bool showIndicators = false, RowRef<TerritoryType> territoryType = default)
    {
        if (key == "Item")
            key += "_" + item.RowId.ToString();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = !(ItemService.IsFish(item) || ItemService.IsSpearfish(item)) && LeveRequiredItems.Any(entry => entry.Item.RowId == item.RowId);

        TextureService.DrawIcon(new GameIconLookup(item.Icon, isLeveRequiredItem), 20);
        ImGui.SameLine();

        var color = Color.White;

        if (ItemService.GetQuantity(item) >= neededCount)
            color = Color.Green;
        else if (ItemService.GetQuantity(item) < neededCount || ItemService.HasAllIngredients(item) == false)
            color = Color.Grey;

        using (ImRaii.PushColor(ImGuiCol.Text, (uint)color))
            ImGui.Selectable($"{(neededCount > 0 ? $"{ItemService.GetQuantity(item)}/{neededCount} " : "")}{ItemService.GetItemName(item)}{(isLeveRequiredItem ? (char)SeIconChar.HighQuality : "")}##{key}_Selectable");

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (ItemService.IsCraftable(item))
            {
                ImGui.SetTooltip(TextService.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (ItemService.IsGatherable(item))
            {
                if (territoryType.IsValid)
                {
                    ImGui.SetTooltip(TextService.GetAddonText(8506)); // "Open Map"
                }
                else
                {
                    ImGui.SetTooltip(TextService.GetAddonText(1472)); // "Search for Item by Gathering Method"
                }
            }
            else if (ItemService.IsFish(item) || ItemService.IsSpearfish(item))
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
                    Color.Grey,
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(Color.Grey, $"https://www.garlandtools.org/db/#item/{item.RowId}");
                ImGui.EndTooltip();
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (ItemService.IsCraftable(item))
            {
                unsafe
                {
                    AgentRecipeNote.Instance()->OpenRecipeByItemId(item.RowId);
                    ImGui.SetWindowFocus(null);
                }
            }
            // TODO: preferance setting?
            else if (ItemService.IsGatherable(item))
            {
                unsafe
                {
                    if (territoryType.IsValid)
                    {
                        var point = ItemService.GetGatheringPoints(item).First(point => point.TerritoryType.RowId == territoryType.RowId);
                        MapService.OpenMap(point, item.AsRef(), "LeveHelper");
                    }
                    else
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                    }

                    ImGui.SetWindowFocus(null);
                }
            }
            else if (ItemService.IsFish(item) && ItemService.GetFishingSpots(item).TryGetFirst(out var spot))
            {
                MapService.OpenMap(spot, item.AsRef(), new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString());
                ImGui.SetWindowFocus(null);
            }
            else if (ItemService.IsSpearfish(item))
            {
                MapService.OpenMap(ItemService.GetSpearfishingGatheringPoints(item).First(), item.AsRef(), new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString());
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
        }

        ImGuiContextMenuService.Draw($"##ItemContextMenu_{key}_Tooltip", (builder) =>
        {
            var itemRef = item.AsRef();
            builder
                .AddSearchCraftingMethod(itemRef)
                .AddSearchGatheringMethod(itemRef)
                .AddOpenInFishGuide(itemRef)
                .AddOpenMapForGatheringPoint(itemRef, territoryType, new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString())
                .AddOpenMapForFishingSpot(itemRef, new SeStringBuilder().Append("LeveHelper").ToReadOnlySeString())
                .AddSeparator()
                .AddItemFinder(item.RowId)
                .AddCopyItemName(item.RowId)
                .AddItemSearch(itemRef)
                .AddOpenOnGarlandTools("item", item.RowId);
        });

        var classJobIcon = 0u;

        if (ItemService.IsCraftable(item))
        {
            classJobIcon = 62008 + ItemService.GetRecipes(item).First().CraftType.RowId;
        }
        else if (ItemService.IsGatherable(item))
        {
            if (ItemService.GetGatheringPoints(item).TryGetFirst(out var point))
            {
                var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
                var rare = !Misc.IsGatheringTypeRare(point.Type);
                classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
            }
        }
        else if (ItemService.IsFish(item))
        {
            if (ItemService.GetFishingSpots(item).TryGetFirst(out var spot))
            {
                classJobIcon = spot.GetFishingSpotIcon();
            }
        }
        else if (ItemService.IsSpearfish(item))
        {
            if (ItemService.GetSpearfishingGatheringPoints(item).TryGetFirst(out var point))
            {
                var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
                var rare = !Misc.IsGatheringTypeRare(point.Type);
                classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
            }
        }

        if (showIndicators && classJobIcon != 0)
        {
            var availSize = ImGui.GetContentRegionMax();

            if (ItemService.IsGatherable(item) && !ItemService.GetGatheringItems(item).Any(gi => !gi.IsHidden))
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
