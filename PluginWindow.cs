using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using LeveHelper.Sheets;
using LeveHelper.Utils;

namespace LeveHelper;

public unsafe class PluginWindow : Window
{
    public const int TextWrapBreakpoint = 820;

    private Vector2 _position;
    private Vector2 _size;

    private readonly QueueTab _queueTab;
    private readonly ConfigurationTab _configurationTab;
    private readonly RecipeTreeTab _recipeTreeTab;
    private readonly ListTab _listTab;

    private ushort[] _lastActiveLevequestIds = Array.Empty<ushort>();

    public PluginWindow() : base("LeveHelper")
    {
        Namespace = "##LeveHelper";

        Size = new Vector2(830, 600);
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(350, 400),
            MaximumSize = new Vector2(4096, 2160)
        };

        _listTab = new(this);
        _queueTab = new(this);
        _recipeTreeTab = new(this);
        _configurationTab = new(this);

        IsOpen = true;
    }

    public RequiredItem[] LeveRequiredItems { get; private set; } = Array.Empty<RequiredItem>();
    public QueuedItem[] RequiredItems { get; private set; } = Array.Empty<QueuedItem>();
    public QueuedItem[] Crystals { get; private set; } = Array.Empty<QueuedItem>();
    public ZoneItems[] Gatherable { get; private set; } = Array.Empty<ZoneItems>();
    public QueuedItem[] OtherSources { get; private set; } = Array.Empty<QueuedItem>();
    public QueuedItem[] Craftable { get; private set; } = Array.Empty<QueuedItem>();

    public override void OnOpen()
    {
        Plugin.StartTown = PlayerState.Instance()->StartTown;
    }

    public override void OnClose()
    {
        Plugin.CloseWindow();
    }

    public void OnAddonOpen(string addonName, AtkUnitBase* unitbase)
    {
        if (addonName is "Catch")
            Refresh();
    }

    public void OnAddonClose(string addonName, AtkUnitBase* unitbase)
    {
        if (addonName is "Synthesis" or "SynthesisSimple" or "Gathering" or "ItemSearchResult" or "InclusionShop" or "Shop" or "ShopExchangeCurrency" or "ShopExchangeItem")
            Refresh();
    }

    private void Refresh()
    {
        UpdateList();
        Plugin.FilterManager.Update();
    }

    public override void PreDraw()
    {
        var activeLevequestIds = Service.GameFunctions.ActiveLevequestsIds;
        if (!_lastActiveLevequestIds.SequenceEqual(activeLevequestIds))
        {
            _lastActiveLevequestIds = activeLevequestIds;

            UpdateList();
            Plugin.FilterManager.Update();

            // TODO: option to auto-open after accepting a levequest
        }
    }

    public override bool DrawConditions()
        => Service.ClientState.IsLoggedIn;

    public override void Draw()
    {
        _position = ImGui.GetWindowPos();
        _size = ImGui.GetWindowSize();

        if (ImGui.BeginTabBar("LeveHelperTabs", ImGuiTabBarFlags.Reorderable))
        {
            if (ImGui.BeginTabItem("Levequest List"))
            {
                RespectCloseHotkey = true;

                _listTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Queue"))
            {
                RespectCloseHotkey = false;

                _queueTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Recipe Tree"))
            {
                RespectCloseHotkey = true;

                _recipeTreeTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Configuration"))
            {
                RespectCloseHotkey = true;

                _configurationTab.Draw();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    public void UpdateList()
    {
        var acceptedCraftAndGatherLeves = Service.GameFunctions.ActiveLevequests
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
                {
                    TraverseItems(item, entry.Amount, neededAmounts);
                }
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
                    var totalAmount = (uint)Math.Ceiling((double)amount * dependency.Amount / dependencyItem.ResultAmount);
                    TraverseItems(dependencyItem, totalAmount, neededAmounts);
                }
            }
        }

        if (newNode)
        {
            neededAmounts.Add(item.RowId, node);
        }
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
                    var ingredientCount = (uint)(ingredientAmount / (double)(ingredientItem.Recipe?.AmountResult ?? 1));
                    DrawIngredients($"{key}_{ingredient.RowId}", ingredientItem.Ingredients, ingredientCount, depth + 1);
                }
            }
        }

        if (depth > 0)
            ImGui.Unindent();
    }

    public void DrawItem(Item item, uint neededCount, string key = "Item", bool showIndicators = false, TerritoryType? territoryType = null)
    {
        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = LeveRequiredItems.Any(entry => entry.Item.RowId == item.RowId);

        Service.TextureCache.GetIcon(item.Icon, isLeveRequiredItem).Draw(20);
        ImGui.SameLine();

        var color = Colors.White;

        if (item.QuantityOwned >= neededCount)
            color = Colors.Green;
        else if (item.QuantityOwned < neededCount || item.HasAllIngredients == false)
            color = Colors.Grey;

        ImGui.PushStyleColor(ImGuiCol.Text, (uint)color);
        ImGui.Selectable($"{item.QuantityOwned}/{neededCount} {item.Name}{(isLeveRequiredItem ? (char)SeIconChar.HighQuality : "")}##{key}_Selectable");
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (item.IsCraftable == true)
            {
                ImGui.SetTooltip(StringUtil.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (item.IsGatherable == true)
            {
                if (territoryType != null)
                {
                    ImGui.SetTooltip(StringUtil.GetAddonText(8506)); // "Open Map"
                }
                else
                {
                    ImGui.SetTooltip(StringUtil.GetAddonText(1472)); // "Search for Item by Gathering Method"
                }
            }
            else if (item.IsFish == true)
            {
                ImGui.SetTooltip(StringUtil.GetAddonText(8506)); // "Open Map"
            }
            else
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();
                ImGui.Text("Open on GarlandTools");

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
                        Service.GameFunctions.OpenMapWithGatheringPoint(point, item);
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
                Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First(), item);
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
        }

        if (ImGui.BeginPopupContextItem($"##ItemContextMenu_{key}_Tooltip"))
        {
            var showSeparator = false;

            if (item.IsCraftable == true)
            {
                if (ImGui.Selectable(StringUtil.GetAddonText(1414))) // "Search for Item by Crafting Method"
                {
                    unsafe
                    {
                        AgentRecipeNote.Instance()->OpenRecipeByItemId(item.RowId);
                        ImGui.SetWindowFocus(null);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                showSeparator = true;
            }

            if (item.IsGatherable == true)
            {
                if (territoryType != null)
                {
                    if (ImGui.Selectable(StringUtil.GetAddonText(8506))) // "Open Map"
                    {
                        var point = item.GatheringPoints.First(point => point.TerritoryType.Row == territoryType.RowId);
                        Service.GameFunctions.OpenMapWithGatheringPoint(point, item);
                        ImGui.SetWindowFocus(null);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }
                }

                if (ImGui.Selectable(StringUtil.GetAddonText(1472))) // "Search for Item by Gathering Method"
                {
                    unsafe
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                        ImGui.SetWindowFocus(null);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                showSeparator = true;
            }

            if (item.IsFish == true)
            {
                if (territoryType != null)
                {
                    if (ImGui.Selectable(StringUtil.GetAddonText(8506))) // "Open Map"
                    {
                        Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First(), item);
                        ImGui.SetWindowFocus(null);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }
                }

                if (ImGui.Selectable("Open in Fish Guide"))
                {
                    unsafe
                    {
                        var agent = (AgentFishGuide*)AgentModule.Instance()->GetAgentByInternalId(AgentId.FishGuide);
                        agent->OpenForItemId(item.RowId, item.IsSpearfishing);
                        ImGui.SetWindowFocus(null);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                showSeparator = true;
            }

            if (showSeparator)
                ImGui.Separator();

            if (ImGui.Selectable(StringUtil.GetAddonText(4379))) // "Search for Item"
            {
                unsafe
                {
                    ItemFinderModule.Instance()->SearchForItem(item.RowId);
                }
                ImGui.SetWindowFocus(null);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }

            if (ImGui.Selectable(StringUtil.GetAddonText(159))) // "Copy Item Name"
            {
                ImGui.SetClipboardText(item.Name);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }

            if (ImGui.Selectable("Open on GarlandTools"))
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();

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

            // TODO: search on market

            ImGui.EndPopup();
        }

        if (showIndicators && item.ClassJobIcon != null)
        {
            var pos = ImGui.GetCursorPos();
            var availSize = ImGui.GetContentRegionAvail();
            ImGui.SameLine(availSize.X - pos.X, 0); // TODO: no -20 here??
            Service.TextureCache.GetIcon((int)item.ClassJobIcon).Draw(20);
        }
    }
}
