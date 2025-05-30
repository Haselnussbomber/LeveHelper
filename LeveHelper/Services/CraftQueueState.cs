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
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using TerritoryType = Lumina.Excel.Sheets.TerritoryType;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public partial class CraftQueueState : IDisposable
{
    private readonly IClientState _clientState;
    private readonly ExcelService _excelService;
    private readonly TextureService _textureService;
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
            .Where(entry => _itemService.GetQueueCategory(entry.Item.RowId) == ItemQueueCategory.Crystals)
            .ToArray();

        // special case for Gatherables
        {
            var gatherables = RequiredItems
                .Where(entry => _itemService.GetQueueCategory(entry.Item.RowId) == ItemQueueCategory.Gatherable)
                .ToArray();

            // group items by zone
            var zones = new Dictionary<uint, HashSet<QueuedItem>>();
            foreach (var entry in gatherables)
            {
                foreach (var point in _itemService.GetGatheringPoints(entry.Item.RowId))
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

                foreach (var spot in _itemService.GetFishingSpots(entry.Item.RowId))
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

                foreach (var point in _itemService.GetSpearfishingGatheringPoints(entry.Item.RowId))
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
                    .ThenBy(zone => zone.Key.CompareTo(_clientState.TerritoryType))
                    .First();

                if (!groupedGatherables.ContainsKey(zone.Key) && _excelService.TryGetRow<TerritoryType>(zone.Key, out var territoryType))
                    groupedGatherables.Add(zone.Key, new(territoryType, zone.Value));
            }

            Gatherable = groupedGatherables.Values.Distinct().ToArray();
        }

        OtherSources = RequiredItems
            .Where(entry => _itemService.GetQueueCategory(entry.Item.RowId) == ItemQueueCategory.OtherSources)
            .ToArray();

        Craftable = RequiredItems
            .Where(entry => _itemService.GetQueueCategory(entry.Item.RowId) == ItemQueueCategory.Craftable)
            .ToArray();
    }

    private void TraverseItems(Item item, uint amount, Dictionary<uint, QueuedItem> neededAmounts)
    {
        var newNode = false;
        if (!neededAmounts.TryGetValue(item.RowId, out var node))
        {
            _itemService.InvalidateQuantity(item.RowId);
            node = new(item, 0, _itemService);
            newNode = true;
        }

        node.AmountNeeded += amount;

        if (node.AmountLeft != 0)
        {
            float resultAmount = _itemService.IsCraftable(item.RowId) ? _itemService.GetRecipes(item.RowId).First().AmountResult : 1;

            foreach (var dependency in _itemService.GetIngredients(item.RowId))
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
        using var indent = ImRaii.PushIndent(1, depth > 0);

        foreach (var entry in ingredients)
        {
            var ingredientAmount = entry.Amount * parentAmount;

            // filter crystals completely if we have enough
            if (_itemService.IsCrystal(entry.Item.RowId) && _itemService.GetQuantity(entry.Item.RowId) >= ingredientAmount)
                continue;

            DrawItem(entry.Item, ingredientAmount, $"{key}_{entry.Item.RowId}");

            // filter ingredients if we have enough
            if (_itemService.GetQuantity(entry.Item.RowId) >= ingredientAmount)
                continue;

            var entryIngredients = _itemService.GetIngredients(entry.Item.RowId);
            if (entryIngredients.Length != 0)
            {
                float resultAmount = _itemService.IsCraftable(entry.Item.RowId) ? _itemService.GetRecipes(entry.Item.RowId).First().AmountResult : 1;
                var ingredientCount = (uint)Math.Ceiling(ingredientAmount / resultAmount);
                DrawIngredients($"{key}_{entry.Item.RowId}", entryIngredients, ingredientCount, depth + 1);
            }
        }
    }

    public void DrawItem(Item item, uint neededCount = 0, string key = "Item", bool showIndicators = false, TerritoryType territoryType = default)
    {
        if (key == "Item")
            key += "_" + item.RowId.ToString();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = !(_itemService.IsFish(item.RowId) || _itemService.IsSpearfish(item.RowId)) && LeveRequiredItems.Any(entry => entry.Item.RowId == item.RowId);

        _textureService.DrawIcon(new GameIconLookup(item.Icon, isLeveRequiredItem), 20);
        ImGui.SameLine();

        var color = Color.White;

        if (neededCount > 0)
        {
            if (_itemService.GetQuantity(item.RowId) >= neededCount)
                color = Color.Green;
            else if (_itemService.GetQuantity(item.RowId) < neededCount || _itemService.HasAllIngredients(item.RowId) == false)
                color = Color.Grey;
        }

        using (ImRaii.PushColor(ImGuiCol.Text, color))
            ImGui.Selectable($"{(neededCount > 0 ? $"{_itemService.GetQuantity(item.RowId)}/{neededCount} " : "")}{_textService.GetItemName(item.RowId)}{(isLeveRequiredItem ? (char)SeIconChar.HighQuality : "")}##{key}_Selectable");

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (_itemService.IsCraftable(item.RowId))
            {
                ImGui.SetTooltip(_textService.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (_itemService.IsGatherable(item.RowId))
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
            else if (_itemService.IsFish(item.RowId) || _itemService.IsSpearfish(item.RowId))
            {
                ImGui.SetTooltip(_textService.GetAddonText(8506)); // "Open Map"
            }
            else
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_textService.Translate("ItemContextMenu.OpenOnGarlandTools"));

                var pos = ImGui.GetCursorPos();
                ImGui.GetWindowDrawList().AddText(
                    UiBuilder.IconFont, 12,
                    ImGui.GetWindowPos() + pos + new Vector2(2),
                    Color.Grey.ToUInt(),
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(Color.Grey, $"https://www.garlandtools.org/db/#item/{item.RowId}");
                ImGui.EndTooltip();
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (_itemService.IsCraftable(item.RowId))
            {
                unsafe
                {
                    AgentRecipeNote.Instance()->SearchRecipeByItemId(item.RowId);
                    ImGui.SetWindowFocus(null);
                }
            }
            // TODO: preferance setting?
            else if (_itemService.IsGatherable(item.RowId))
            {
                unsafe
                {
                    if (territoryType.RowId != 0)
                    {
                        var point = _itemService.GetGatheringPoints(item.RowId).First(point => point.TerritoryType.RowId == territoryType.RowId);
                        _mapService.OpenMap(point, item.RowId, "LeveHelper");
                    }
                    else
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                    }

                    ImGui.SetWindowFocus(null);
                }
            }
            else if (_itemService.IsFish(item.RowId) && _itemService.GetFishingSpots(item.RowId).TryGetFirst(out var spot))
            {
                _mapService.OpenMap(spot, item.RowId, "LeveHelper");
                ImGui.SetWindowFocus(null);
            }
            else if (_itemService.IsSpearfish(item.RowId))
            {
                _mapService.OpenMap(_itemService.GetSpearfishingGatheringPoints(item.RowId).First(), item.RowId, "LeveHelper");
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
        }

        _imGuiContextMenuService.Draw($"##ItemContextMenu_{key}_Tooltip", (builder) =>
        {
            builder
                .AddSearchCraftingMethod(item.RowId)
                .AddSearchGatheringMethod(item.RowId)
                .AddOpenInFishGuide(item.RowId)
                .AddOpenMapForGatheringPoint(item.RowId, territoryType, "LeveHelper")
                .AddOpenMapForFishingSpot(item.RowId, "LeveHelper")
                .AddSeparator()
                .AddItemFinder(item.RowId)
                .AddCopyItemName(item.RowId)
                .AddItemSearch(item.RowId)
                .AddOpenOnGarlandTools("item", item.RowId);
        });

        var classJobIcon = 0u;

        if (_itemService.IsCraftable(item.RowId))
        {
            classJobIcon = 62008 + _itemService.GetRecipes(item.RowId).First().CraftType.RowId;
        }
        else if (_itemService.IsGatherable(item.RowId))
        {
            if (_itemService.GetGatheringPoints(item.RowId).TryGetFirst(out var point))
            {
                var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
                var rare = !UIGlobals.IsExportedGatheringPointTimed(point.Type);
                classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
            }
        }
        else if (_itemService.IsFish(item.RowId))
        {
            if (_itemService.GetFishingSpots(item.RowId).TryGetFirst(out var spot))
                classJobIcon = spot.GetFishingSpotIcon();
        }
        else if (_itemService.IsSpearfish(item.RowId))
        {
            if (_itemService.GetSpearfishingGatheringPoints(item.RowId).TryGetFirst(out var point))
            {
                var gatheringType = point.GatheringPointBase.Value!.GatheringType.Value!;
                var rare = !UIGlobals.IsExportedGatheringPointTimed(point.Type);
                classJobIcon = rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
            }
        }

        if (showIndicators && classJobIcon != 0)
        {
            var availSize = ImGui.GetContentRegionMax();

            if (_itemService.IsGatherable(item.RowId) && !_itemService.GetGatheringItems(item.RowId).Any(gi => !gi.IsHidden))
            {
                var text = _textService.GetAddonText(627); // or 629? "Hidden"
                var textWidth = ImGui.CalcTextSize(text).X;
                ImGui.SameLine(availSize.X - textWidth - ImGui.GetStyle().ItemInnerSpacing.X - 20, 0);
                ImGui.TextUnformatted(text);
            }

            ImGui.SameLine(availSize.X - 20, 0);
            _textureService.DrawIcon(classJobIcon, 20);
        }
    }
}
