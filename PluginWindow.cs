using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace LeveHelper;

public unsafe class PluginWindow : Window
{
    public const int TextWrapBreakpoint = 820;

    public Vector2 MyPosition { get; private set; }
    public Vector2 MySize { get; private set; }
    public RequiredItem[] LeveRequiredItems { get; private set; } = Array.Empty<RequiredItem>();
    public QueuedItem[] RequiredItems { get; private set; } = Array.Empty<QueuedItem>();
    public QueuedItem[] Crystals { get; private set; } = Array.Empty<QueuedItem>();
    public ZoneItems[] Gatherable { get; private set; } = Array.Empty<ZoneItems>();
    public QueuedItem[] OtherSources { get; private set; } = Array.Empty<QueuedItem>();
    public QueuedItem[] Craftable { get; private set; } = Array.Empty<QueuedItem>();

    private readonly QueueTab QueueTab;
    private readonly ConfigurationTab ConfigurationTab;
    private readonly RecipeTreeTab RecipeTreeTab;
    private readonly ListTab ListTab;

    private readonly AddonObserver CatchObserver = new("Catch");
    private readonly AddonObserver SynthesisObserver = new("Synthesis");
    private readonly AddonObserver SynthesisSimpleObserver = new("SynthesisSimple");
    private readonly AddonObserver GatheringObserver = new("Gathering");
    private readonly AddonObserver ShopObserver = new("Shop");
    private readonly AddonObserver ItemSearchResultObserver = new("ItemSearchResult");

    private ushort[] LastActiveLevequestIds = Array.Empty<ushort>();

    public PluginWindow() : base("LeveHelper")
    {
        base.Size = new Vector2(830, 600);
        base.SizeCondition = ImGuiCond.FirstUseEver;
        base.SizeConstraints = new()
        {
            MinimumSize = new Vector2(350, 400),
            MaximumSize = new Vector2(4096, 2160)
        };

        ListTab = new(this);
        QueueTab = new(this);
        RecipeTreeTab = new(this);
        ConfigurationTab = new(this);
    }

    public override void OnOpen()
    {
        Plugin.FilterManager ??= new();

        CatchObserver.OnOpen += Refresh;
        SynthesisObserver.OnClose += Refresh;
        SynthesisSimpleObserver.OnClose += Refresh;
        GatheringObserver.OnClose += Refresh;
        ShopObserver.OnClose += Refresh;
        ItemSearchResultObserver.OnClose += Refresh;
    }

    public override void OnClose()
    {
        CatchObserver.OnOpen -= Refresh;
        SynthesisObserver.OnClose -= Refresh;
        SynthesisSimpleObserver.OnClose -= Refresh;
        GatheringObserver.OnClose -= Refresh;
        ShopObserver.OnClose -= Refresh;
        ItemSearchResultObserver.OnClose -= Refresh;
    }

    private void Refresh(AddonObserver sender, AtkUnitBase* unitBase)
    {
        UpdateList();
    }

    public override void PreDraw()
    {
        CatchObserver.Update();
        SynthesisObserver.Update();
        SynthesisSimpleObserver.Update();
        GatheringObserver.Update();
        ShopObserver.Update();
        ItemSearchResultObserver.Update();

        var activeLevequestIds = Service.GameFunctions.ActiveLevequestsIds;
        if (!LastActiveLevequestIds.SequenceEqual(activeLevequestIds))
        {
            LastActiveLevequestIds = activeLevequestIds;

            UpdateList();
        }
    }

    public override bool DrawConditions()
    {
        return Service.ClientState.IsLoggedIn;
    }

    public override void Draw()
    {
        MyPosition = ImGui.GetWindowPos();
        MySize = ImGui.GetWindowSize();

        if (ImGui.BeginTabBar("LeveHelperTabs", ImGuiTabBarFlags.Reorderable))
        {
            if (ImGui.BeginTabItem("Levequest List"))
            {
                RespectCloseHotkey = true;

                ListTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Queue"))
            {
                RespectCloseHotkey = false;

                QueueTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Recipe Tree"))
            {
                RespectCloseHotkey = true;

                RecipeTreeTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Configuration"))
            {
                RespectCloseHotkey = true;

                ConfigurationTab.Draw();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    public void UpdateList()
    {
        var acceptedCraftAndGatherLeves = Service.GameFunctions.ActiveLevequests
            .Where(leve => (leve.IsCraftLeve || leve.IsGatherLeve) && leve.RequiredItems != null);

        // list all items required for CraftLeves and GatherLeves
        LeveRequiredItems = acceptedCraftAndGatherLeves
            .SelectMany(leve => leve.RequiredItems!)
            .ToArray();

        // gather a list of all items needed to craft everything
        var neededAmounts = new Dictionary<uint, QueuedItem>();
        foreach (var leve in acceptedCraftAndGatherLeves)
        {
            foreach (var entry in leve.RequiredItems!)
            {
                TraverseItems(entry.Item, entry.Amount, neededAmounts);
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
                    if (zones.TryGetValue(point.TerritoryTypeId, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(point.TerritoryTypeId, new() { entry });
                    }
                }

                foreach (var spot in entry.Item.FishingSpots)
                {
                    if (zones.TryGetValue(spot.TerritoryTypeId, out var list))
                    {
                        list.Add(entry);
                    }
                    else
                    {
                        zones.Add(spot.TerritoryTypeId, new() { entry });
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
                    .ThenBy(zone => zone.Key.CompareTo(Service.ClientState.TerritoryType))
                    .First();

                if (!groupedGatherables.ContainsKey(zone.Key))
                    groupedGatherables.Add(zone.Key, new(TerritoryTypeCache.Get(zone.Key), zone.Value));
            }

            // sorting by cheapest teleport costs
            var zoneItems = groupedGatherables.Values.ToList();
            zoneItems.Insert(0, new(TerritoryTypeCache.Get(Service.ClientState.TerritoryType), new())); // add starting zone

            var nodes = new Dictionary<(ZoneItems, ZoneItems), uint>();
            foreach (var zoneItemFrom in zoneItems)
            {
                foreach (var zoneItemTo in zoneItems)
                {
                    var cost = Service.GameFunctions.CalculateTeleportCost(zoneItemFrom.TerritoryType.RowId, zoneItemTo.TerritoryType.RowId, false, false, false);
                    nodes.Add((zoneItemFrom, zoneItemTo), cost);
                }
            }

            Gatherable = nodes
                .OrderBy(kv => kv.Value) // sort by cost
                .Where(kv => kv.Key.Item2.Items.Count != 0) // filter starting zone
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

    private static void TraverseItems(CachedItem item, uint amount, Dictionary<uint, QueuedItem> neededAmounts)
    {
        var newNode = false;
        if (!neededAmounts.TryGetValue(item.ItemId, out var node))
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
                var totalAmount = (uint)Math.Ceiling((double)amount * dependency.Amount / dependency.Item.ResultAmount);
                TraverseItems(dependency.Item, totalAmount, neededAmounts);
            }
        }

        if (newNode)
        {
            neededAmounts.Add(item.ItemId, node);
        }
    }
}
