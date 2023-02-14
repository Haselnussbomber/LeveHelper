using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using static LeveHelper.ImGuiUtils;

namespace LeveHelper;

public class CraftingHelperWindow : Window
{
    private readonly AddonObserver CatchObserver = new("Catch");
    private readonly AddonObserver SynthesisObserver = new("Synthesis");
    private readonly AddonObserver SynthesisSimpleObserver = new("SynthesisSimple");
    private readonly AddonObserver GatheringObserver = new("Gathering");
    private readonly AddonObserver ShopObserver = new("Shop");

    private RequiredItem[] LeveRequiredItems = Array.Empty<RequiredItem>();
    private QueuedItem[] RequiredItems = Array.Empty<QueuedItem>();

    private QueuedItem[] Crystals = Array.Empty<QueuedItem>();
    private (CachedTerritoryType, HashSet<QueuedItem>)[] Gatherable = Array.Empty<(CachedTerritoryType, HashSet<QueuedItem>)>();
    private QueuedItem[] Fishable = Array.Empty<QueuedItem>();
    private QueuedItem[] OtherSources = Array.Empty<QueuedItem>();
    private QueuedItem[] Craftable = Array.Empty<QueuedItem>();

    private ushort[] LastActiveLevequestIds = Array.Empty<ushort>();

    public CraftingHelperWindow() : base("LeveHelper - Crafting Helper")
    {
        base.RespectCloseHotkey = false;

        base.Size = new Vector2(350, Plugin.PluginWindow.MySize.Y);
        base.SizeCondition = ImGuiCond.Appearing;
        base.SizeConstraints = new()
        {
            MinimumSize = new Vector2(320, 400),
            MaximumSize = new Vector2(4096, 2160)
        };

        base.PositionCondition = ImGuiCond.FirstUseEver;
        base.Position = Plugin.PluginWindow.MyPosition + new Vector2(Plugin.PluginWindow.MySize.X, 0);
    }

    public override unsafe void OnOpen()
    {
        Plugin.FilterManager ??= new();

        CatchObserver.OnOpen += Refresh;
        SynthesisObserver.OnClose += Refresh;
        SynthesisSimpleObserver.OnClose += Refresh;
        GatheringObserver.OnClose += Refresh;
        ShopObserver.OnClose += Refresh;
    }

    public override unsafe void OnClose()
    {
        CatchObserver.OnOpen -= Refresh;
        SynthesisObserver.OnClose -= Refresh;
        SynthesisSimpleObserver.OnClose -= Refresh;
        GatheringObserver.OnClose -= Refresh;
        ShopObserver.OnClose -= Refresh;
    }

    private unsafe void Refresh(AddonObserver sender, AtkUnitBase* unitBase)
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
        return Service.GameFunctions.ActiveLevequestsIds.Length > 0; // TODO: add this to settings (auto show/hide)
    }

    private void UpdateList()
    {
        var acceptedCraftLeves = Service.GameFunctions.ActiveLevequests
            .Where(leve => leve.IsCraftLeve && leve.RequiredItems != null);

        // step 1: list all items required for craftleves
        LeveRequiredItems = acceptedCraftLeves
            .SelectMany(leve => leve.RequiredItems!)
            .ToArray();

        // step 2: gather a list of all items
        var allItems = new Dictionary<uint, QueuedItem>();
        foreach (var leve in acceptedCraftLeves)
        {
            foreach (var entry in leve.RequiredItems!)
            {
                GetItemsRecursive(allItems, entry, 1);
            }
        }

        // step 3: decrease amount out ingredients for completed items
        foreach (var leve in acceptedCraftLeves)
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
            }

            // for each item, get the one zone with the most items in it
            var groupedGatherables = new Dictionary<uint, (CachedTerritoryType, HashSet<QueuedItem>)>();
            foreach (var entry in gatherables)
            {
                var zone = zones
                    .Where(zone => zone.Value.Contains(entry))
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

        Fishable = RequiredItems
            .Where(entry => entry.Item.QueueCategory == ItemQueueCategory.Fishable)
            .ToArray();

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

    public override void Draw()
    {
        if (ImGuiComponents.IconButton(FontAwesomeIcon.StickyNote))
        {
            Plugin.PluginWindow.IsOpen = true;
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.SetTooltip("Show LeveHelper");
        }

        ImGui.SameLine();

        if (ImGuiComponents.IconButton(FontAwesomeIcon.RedoAlt))
        {
            UpdateList();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.SetTooltip("Refresh");
        }

        if (ImGui.BeginTabBar("##TabBar"))
        {
            if (ImGui.BeginTabItem("Queue"))
            {
                DrawQueue();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Recipe Tree"))
            {
                DrawIngredients("RecipeTree", LeveRequiredItems, 1);
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    private void DrawQueue()
    {
        var i = 0;

        if (RequiredItems.Any())
        {
            if (Crystals.Any())
            {
                ImGui.Text("Crystals:");
                foreach (var entry in Crystals)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}");
                }
            }

            if (Gatherable.Any())
            {
                ImGui.Text("Gather:");
                foreach (var kv in Gatherable)
                {
                    ImGui.Text(kv.Item1.PlaceName);

                    foreach (var entry in kv.Item2)
                    {
                        DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}");
                    }
                }
            }

            if (Fishable.Any())
            {
                ImGui.Text("Fish:");
                foreach (var entry in Fishable)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}");
                }
            }

            if (OtherSources.Any())
            {
                ImGui.Text("Other:");
                foreach (var entry in OtherSources)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}");
                }
            }

            if (Craftable.Any())
            {
                ImGui.Text("Craft:");
                foreach (var entry in Craftable)
                {
                    DrawItem(entry.Item, entry.AmountLeft, $"Item{i++}");
                }
            }

            ImGui.Separator();

            foreach (var entry in LeveRequiredItems)
            {
                DrawItem(entry.Item, entry.Amount, $"Item{i++}");
            }
        }
    }
}
