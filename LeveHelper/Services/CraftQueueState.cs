using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AutoCtor;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Extensions;
using HaselCommon.Graphics;
using HaselCommon.Services;
using HaselCommon.Utils;
using LeveHelper.Enums;
using LeveHelper.Records;
using Lumina.Extensions;
using TerritoryType = Lumina.Excel.Sheets.TerritoryType;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public partial class CraftQueueState : IDisposable
{
    private readonly IClientState _clientState;
    private readonly ITextureProvider _textureProvider;
    private readonly ExcelService _excelService;
    private readonly TextService _textService;
    private readonly MapService _mapService;
    private readonly ImGuiContextMenuService _imGuiContextMenuService;
    private readonly LeveService _leveService;
    private readonly ExtendedItemService _itemService;
    private readonly AddonObserver _addonObserver;
    private readonly IGameInventory _gameInventory;
    private readonly IFramework _framework;
    private ushort[] _lastActiveLevequestIds = [];

    [AutoPostConstruct]
    private void Initialize()
    {
        _addonObserver.AddonOpen += OnAddonOpen;
        _addonObserver.AddonClose += OnAddonClose;
        _gameInventory.InventoryChangedRaw += OnInventoryChangedRaw;
        _framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        _framework.Update -= OnFrameworkUpdate;
        _gameInventory.InventoryChangedRaw -= OnInventoryChangedRaw;
        _addonObserver.AddonOpen -= OnAddonOpen;
        _addonObserver.AddonClose -= OnAddonClose;

        GC.SuppressFinalize(this);
    }

    private void OnInventoryChangedRaw(IReadOnlyCollection<InventoryEventArgs> events)
    {
        if (!_clientState.IsLoggedIn)
            return;

        UpdateList();
    }

    private void OnAddonOpen(string addonName)
    {
        if (!_clientState.IsLoggedIn)
            return;

        if (addonName is "Catch")
            UpdateList();
    }

    private void OnAddonClose(string addonName)
    {
        if (!_clientState.IsLoggedIn)
            return;

        if (addonName is "Synthesis" or "SynthesisSimple" or "Gathering" or "ItemSearchResult" or "InclusionShop" or "Shop" or "ShopExchangeCurrency" or "ShopExchangeItem")
            UpdateList();
    }

    public void OnFrameworkUpdate(IFramework framework)
    {
        if (!_clientState.IsLoggedIn)
            return;

        var activeLevequestIds = _leveService.GetActiveLeveIds().ToArray();
        if (!_lastActiveLevequestIds.SequenceEqual(activeLevequestIds))
        {
            _lastActiveLevequestIds = activeLevequestIds;
            UpdateList();
        }
    }

    public ItemAmount[] LeveRequiredItems { get; private set; } = [];
    public QueuedItem[] RequiredItems { get; private set; } = [];
    public QueuedItem[] Crystals { get; private set; } = [];
    public ZoneItems[] Gatherable { get; private set; } = [];
    public QueuedItem[] OtherSources { get; private set; } = [];
    public QueuedItem[] Craftable { get; private set; } = [];

    public void UpdateList()
    {
        var acceptedCraftAndGatherLeves = _leveService.GetActiveLeves()
            .Select(leve => (Leve: leve, Items: _leveService.GetRequiredItems(leve.RowId)))
            .Where(items => items.Items.Length > 0);

        // list all items required for CraftLeves and GatherLeves
        LeveRequiredItems = acceptedCraftAndGatherLeves
            .SelectMany(entry => entry.Items)
            .ToArray();

        // gather a list of all items needed to craft everything
        var neededAmounts = new Dictionary<uint, QueuedItem>();
        foreach (var (_, Items) in acceptedCraftAndGatherLeves)
        {
            foreach (var itemAmount in Items)
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
            .Where(entry => _itemService.GetQueueCategory(entry.Item) == ItemQueueCategory.Crystals)
            .ToArray();

        // special case for Gatherables
        {
            var gatherables = RequiredItems
                .Where(entry => _itemService.GetQueueCategory(entry.Item) == ItemQueueCategory.Gatherable)
                .ToArray();

            // group items by zone
            var zones = new Dictionary<uint, HashSet<QueuedItem>>();
            foreach (var entry in gatherables)
            {
                foreach (var point in _itemService.GetGatheringPoints(entry.Item))
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

                foreach (var spot in _itemService.GetFishingSpots(entry.Item))
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

                foreach (var point in _itemService.GetSpearfishingGatheringPoints(entry.Item))
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
                    .Where(zone => zone.Value.Select(e => e.Item.ItemId).Contains(entry.Item.ItemId))
                    .OrderByDescending(zone => zone.Value.Count)
                    .ThenBy(zone => zone.Key.CompareTo(_clientState.TerritoryType))
                    .First();

                if (!groupedGatherables.ContainsKey(zone.Key) && _excelService.TryGetRow<TerritoryType>(zone.Key, out var territoryType))
                    groupedGatherables.Add(zone.Key, new(territoryType, zone.Value));
            }

            Gatherable = [.. groupedGatherables.Values.Distinct()];
        }

        OtherSources = RequiredItems
            .Where(entry => _itemService.GetQueueCategory(entry.Item) == ItemQueueCategory.OtherSources)
            .ToArray();

        Craftable = RequiredItems
            .Where(entry => _itemService.GetQueueCategory(entry.Item) == ItemQueueCategory.Craftable)
            .ToArray();
    }

    private void TraverseItems(ItemHandle item, uint amount, Dictionary<uint, QueuedItem> neededAmounts)
    {
        var newNode = false;
        if (!neededAmounts.TryGetValue(item, out var node))
        {
            _itemService.InvalidateQuantity(item);
            node = new(item, 0, _itemService);
            newNode = true;
        }

        node.AmountNeeded += amount;

        if (node.AmountLeft != 0)
        {
            float resultAmount = item.IsCraftable ? _itemService.GetRecipes(item)[0].AmountResult : 1;

            foreach (var dependency in _itemService.GetIngredients(item))
            {
                var totalAmount = (uint)Math.Ceiling(amount / resultAmount) * dependency.Amount;
                TraverseItems(dependency.Item, totalAmount, neededAmounts);
            }
        }

        if (newNode)
            neededAmounts.Add(item, node);
    }

    public void DrawIngredients(string key, IReadOnlyList<ItemAmount> ingredients, uint parentAmount = 1, int depth = 0)
    {
        using var indent = ImRaii.PushIndent(1, depth > 0);

        foreach (var entry in ingredients)
        {
            var ingredientAmount = entry.Amount * parentAmount;

            // filter crystals completely if we have enough
            if (entry.Item.IsCrystal && _itemService.GetQuantity(entry.Item) >= ingredientAmount)
                continue;

            DrawItem(entry.Item, ingredientAmount, $"{key}_{entry.Item.ItemId}");

            // filter ingredients if we have enough
            if (_itemService.GetQuantity(entry.Item) >= ingredientAmount)
                continue;

            var entryIngredients = _itemService.GetIngredients(entry.Item);
            if (entryIngredients.Count != 0)
            {
                float resultAmount = _itemService.IsCraftable(entry.Item) ? _itemService.GetRecipes(entry.Item)[0].AmountResult : 1;
                var ingredientCount = (uint)Math.Ceiling(ingredientAmount / resultAmount);
                DrawIngredients($"{key}_{entry.Item.ItemId}", entryIngredients, ingredientCount, depth + 1);
            }
        }
    }

    public void DrawItem(ItemHandle item, uint neededCount = 0, string key = "Item", bool showIndicators = false, TerritoryType territoryType = default)
    {
        if (key == "Item")
            key += "_" + item.ItemId.ToString();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = !(item.IsFish || item.IsSpearfish) && LeveRequiredItems.Any(entry => entry.Item.ItemId == item.ItemId);

        _textureProvider.DrawIcon(new GameIconLookup(item.Icon, isLeveRequiredItem), 20);
        ImGui.SameLine();

        var color = Color.White;

        if (neededCount > 0)
        {
            if (_itemService.GetQuantity(item) >= neededCount)
                color = Color.Green;
            else if (_itemService.GetQuantity(item) < neededCount || _itemService.HasAllIngredients(item) == false)
                color = Color.Grey;
        }

        using (ImRaii.PushColor(ImGuiCol.Text, color))
            ImGui.Selectable($"{(neededCount > 0 ? $"{_itemService.GetQuantity(item)}/{neededCount} " : "")}{item.Name}{(isLeveRequiredItem ? (char)SeIconChar.HighQuality : "")}##{key}_Selectable");

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (item.IsCraftable)
            {
                ImGui.SetTooltip(_textService.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (item.IsGatherable)
            {
                if (territoryType.RowId != 0)
                {
                    ImGui.SetTooltip(_textService.GetAddonText(8506)); // "Open Map"
                }
                else
                {
                    ImGui.SetTooltip(_textService.GetAddonText(1472)); // "Search for Item by Gathering Method"
                }
            }
            else if (item.IsFish || item.IsSpearfish)
            {
                ImGui.SetTooltip(_textService.GetAddonText(8506)); // "Open Map"
            }
            else
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();
                ImGui.Text(_textService.Translate("ItemContextMenu.OpenOnGarlandTools"));

                var pos = ImGui.GetCursorPos();
                ImGui.GetWindowDrawList().AddText(
                    UiBuilder.IconFont, 12,
                    ImGui.GetWindowPos() + pos + new Vector2(2),
                    Color.Grey.ToUInt(),
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(Color.Grey, $"https://www.garlandtools.org/db/#item/{item.ItemId}");
                ImGui.EndTooltip();
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (item.IsCraftable)
            {
                unsafe
                {
                    AgentRecipeNote.Instance()->SearchRecipeByItemId(item);
                    ImGui.ClearWindowFocus();
                }
            }
            // TODO: preferance setting?
            else if (item.IsGatherable)
            {
                unsafe
                {
                    if (territoryType.RowId != 0)
                    {
                        var point = _itemService.GetGatheringPoints(item).First(point => point.TerritoryType.RowId == territoryType.RowId);
                        _mapService.OpenMap(point, item, "LeveHelper");
                    }
                    else
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.ItemId);
                    }

                    ImGui.ClearWindowFocus();
                }
            }
            else if (item.IsFish && _itemService.GetFishingSpots(item).TryGetFirst(out var spot))
            {
                _mapService.OpenMap(spot, item.ItemId, "LeveHelper");
                ImGui.ClearWindowFocus();
            }
            else if (item.IsSpearfish)
            {
                _mapService.OpenMap(_itemService.GetSpearfishingGatheringPoints(item)[0], item.ItemId, "LeveHelper");
                ImGui.ClearWindowFocus();
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.ItemId}"));
            }
        }

        _imGuiContextMenuService.Draw($"##ItemContextMenu_{key}_Tooltip", (builder) =>
        {
            builder
                .AddSearchCraftingMethod(item)
                .AddSearchGatheringMethod(item)
                .AddOpenInFishGuide(item)
                .AddOpenMapForGatheringPoint(item, territoryType, "LeveHelper")
                .AddOpenMapForFishingSpot(item, "LeveHelper")
                .AddSeparator()
                .AddItemFinder(item)
                .AddCopyItemName(item)
                .AddItemSearch(item)
                .AddOpenOnGarlandTools("item", item);
        });

        var classJobIcon = 0u;

        if (item.IsCraftable)
        {
            classJobIcon = 62008 + _itemService.GetRecipes(item.ItemId)[0].CraftType.RowId;
        }
        else if (item.IsGatherable)
        {
            if (_itemService.GetGatheringPoints(item).TryGetFirst(out var point))
            {
                var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
                var rare = !UIGlobals.IsExportedGatheringPointTimed(point.Type);
                classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
            }
        }
        else if (item.IsFish)
        {
            if (_itemService.GetFishingSpots(item).TryGetFirst(out var spot))
                classJobIcon = spot.FishingSpotIcon;
        }
        else if (item.IsSpearfish)
        {
            if (_itemService.GetSpearfishingGatheringPoints(item).TryGetFirst(out var point))
            {
                var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
                var rare = !UIGlobals.IsExportedGatheringPointTimed(point.Type);
                classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
            }
        }

        if (showIndicators && classJobIcon != 0)
        {
            var availSize = ImGui.GetContentRegionMax();

            if (item.IsGatherable && !_itemService.GetGatheringItems(item).Any(gi => !gi.IsHidden))
            {
                var text = _textService.GetAddonText(627); // or 629? "Hidden"
                var textWidth = ImGui.CalcTextSize(text).X;
                ImGui.SameLine(availSize.X - textWidth - ImGui.GetStyle().ItemInnerSpacing.X - 20, 0);
                ImGui.Text(text);
            }

            ImGui.SameLine(availSize.X - 20, 0);
            _textureProvider.DrawIcon(classJobIcon, 20);
        }
    }
}
