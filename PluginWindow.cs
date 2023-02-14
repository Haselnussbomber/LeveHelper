using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
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
    public (CachedTerritoryType, HashSet<QueuedItem>)[] Gatherable { get; private set; } = Array.Empty<(CachedTerritoryType, HashSet<QueuedItem>)>();
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
    }

    public override void OnClose()
    {
        CatchObserver.OnOpen -= Refresh;
        SynthesisObserver.OnClose -= Refresh;
        SynthesisSimpleObserver.OnClose -= Refresh;
        GatheringObserver.OnClose -= Refresh;
        ShopObserver.OnClose -= Refresh;
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

        // step 1: list all items required for craftleves
        LeveRequiredItems = acceptedCraftAndGatherLeves
            .SelectMany(leve => leve.RequiredItems!)
            .ToArray();

        // step 2: gather a list of all items
        var allItems = new Dictionary<uint, QueuedItem>();
        foreach (var leve in acceptedCraftAndGatherLeves)
        {
            foreach (var entry in leve.RequiredItems!)
            {
                GetItemsRecursive(allItems, entry, 1);
            }
        }

        // step 3: decrease amount out ingredients for completed items
        foreach (var leve in acceptedCraftAndGatherLeves)
        {
            foreach (var entry in leve.RequiredItems!)
            {
                FilterItem(allItems, entry);
            }
        }

        RequiredItems = allItems
            .Values
            .Where(entry => entry.AmountLeft > 0) // step 4: filter every completed item
            .ToArray();

        // step 4: categorize
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
            var groupedGatherables = new Dictionary<uint, (CachedTerritoryType, HashSet<QueuedItem>)>();
            foreach (var entry in gatherables)
            {
                var zone = zones
                    .Where(zone => zone.Value.Select(e => e.Item.ItemId).Contains(entry.Item.ItemId))
                    .OrderByDescending(zone => zone.Value.Count)
                    .First();

                if (!groupedGatherables.ContainsKey(zone.Key))
                    groupedGatherables.Add(zone.Key, (TerritoryTypeCache.Get(zone.Key), zone.Value));
            }

            // sorting by cheapest teleport costs
            var array = groupedGatherables.Values.ToList();

            // add starting point
            array.Insert(0, (TerritoryTypeCache.Get(Service.ClientState.TerritoryType), new()));

            // sort by teleport cost
            array.Sort((a, b) => (int)Service.GameFunctions.CalculateTeleportCost(a.Item1.RowId, b.Item1.RowId, false, false, false));

            Gatherable = array
                .Where(entry => entry.Item2.Count != 0) // remove starting point
                .ToArray();
        }

        OtherSources = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.OtherSources)
            .ToArray();

        Craftable = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.Craftable)
            .ToArray();
    }

    private void GetItemsRecursive(Dictionary<uint, QueuedItem> dict, RequiredItem node, uint parentTotalAmount)
    {
        var nodeAmount = node.Amount * parentTotalAmount;
        var resultAmount = (uint)(nodeAmount / (double)(node.Item.Recipe?.AmountResult ?? 1));

        // process ingredients
        if (node.Item.IsCraftable)
        {
            foreach (var ingredient in node.Item.Ingredients!)
            {
                GetItemsRecursive(dict, ingredient, resultAmount);
            }
        }

        // add node to list
        if (dict.TryGetValue(node.Item.ItemId, out var item))
        {
            item.AddAmount(resultAmount);
        }
        else
        {
            dict.Add(node.Item.ItemId, new QueuedItem(node.Item, resultAmount));
            node.Item.UpdateQuantityOwned();
        }
    }

    private void FilterItem(Dictionary<uint, QueuedItem> dict, RequiredItem node)
    {
        // do we have enough?
        if (node.Item.QuantityOwned >= node.Amount)
        {
            // then subtract it from out list
            if (dict.TryGetValue(node.Item.ItemId, out var item))
            {
                item.AmountLeft -= node.Amount;
            }

            // and reduce needed ingredients count
            foreach (var ingredient in node.Item.Ingredients)
            {
                DecreaseIngredientsRecursive(dict, ingredient, node.Amount);
            }
        }
        else
        {
            foreach (var ingredient in node.Item.Ingredients)
            {
                FilterItem(dict, ingredient);
            }
        }
    }

    private void DecreaseIngredientsRecursive(Dictionary<uint, QueuedItem> dict, RequiredItem node, uint parentAmount)
    {
        if (dict.TryGetValue(node.Item.ItemId, out var item))
        {
            item.AmountLeft -= (uint)(node.Amount * parentAmount / (double)(node.Item.Recipe?.AmountResult ?? 1));
        }

        foreach (var ingredient in node.Item.Ingredients)
        {
            DecreaseIngredientsRecursive(dict, ingredient, node.Amount * parentAmount);
        }
    }
}
