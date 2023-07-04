using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using LeveHelper.Sheets;

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
        Plugin.StartTown = PlayerState.Instance()->StartTown;
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
        if (!LastActiveLevequestIds.SequenceEqual(activeLevequestIds))
        {
            LastActiveLevequestIds = activeLevequestIds;

            UpdateList();
            Plugin.FilterManager.Update();

            // TODO: option to auto-open after accepting a levequest
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
                    groupedGatherables.Add(zone.Key, new(Service.Data.GetExcelSheet<TerritoryType>()!.GetRow(zone.Key)!, zone.Value));
            }

            // sorting by cheapest teleport costs
            var zoneItems = groupedGatherables.Values.ToList();
            zoneItems.Insert(0, new(Service.Data.GetExcelSheet<TerritoryType>()!.GetRow(Service.ClientState.TerritoryType)!, new())); // add starting zone

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
}
