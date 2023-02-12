using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
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
    private RequiredItem[] RequiredItems = Array.Empty<RequiredItem>();

    private RequiredItem[] Crystals = Array.Empty<RequiredItem>();
    private RequiredItem[] Gatherable = Array.Empty<RequiredItem>();
    private RequiredItem[] Fishable = Array.Empty<RequiredItem>();
    private RequiredItem[] OtherSources = Array.Empty<RequiredItem>();
    private RequiredItem[] Craftable = Array.Empty<RequiredItem>();

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
        RefreshQuantities();
    }

    private void RefreshQuantities()
    {
        var arr = new Dictionary<uint, CachedItem>();

        var addItems = (RequiredItem[] items) =>
        {
            foreach (var entry in items)
            {
                if (!arr.ContainsKey(entry.Item.ItemId))
                    arr.Add(entry.Item.ItemId, entry.Item);
            }
        };

        addItems(Crystals);
        addItems(Gatherable);
        addItems(Fishable);
        addItems(OtherSources);
        addItems(Craftable);
        addItems(RequiredItems);

        foreach (var item in arr.Values)
            item.UpdateQuantityOwned();

        // TODO: cooldown?
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

            var acceptedCraftLeves = Service.GameFunctions.ActiveLevequests
                .Where(leve => leve.IsCraftLeve);

            var deepestDepth = 0;

            foreach (var leve in acceptedCraftLeves)
            {
                if (leve.RequiredItems == null)
                    continue;

                foreach (var entry in leve.RequiredItems)
                {
                    var depth = CalculateDepth(entry, 0);
                    if (deepestDepth < depth)
                        deepestDepth = depth;
                }
            }

            var sorted = new Dictionary<uint, RequiredItem>();

            foreach (var leve in acceptedCraftLeves)
            {
                if (leve.RequiredItems == null)
                    continue;

                foreach (var entry in leve.RequiredItems)
                {
                    for (var level = deepestDepth; level > 0; level--)
                    {
                        var items = GetItemsAtDepth(entry, 1, level, entry.Amount);

                        foreach (var item in items)
                        {
                            if (!sorted.ContainsKey(item.Item.ItemId))
                            {
                                sorted.Add(item.Item.ItemId, new(item.Item, item.Amount));
                            }
                            else
                            {
                                sorted[item.Item.ItemId].Amount += item.Amount;
                            }
                        }
                    }
                }
            }

            RequiredItems = sorted.Values.ToArray();

            Crystals = RequiredItems
                .Where(entry => entry.Item.IsCrystal && entry.Item.QuantityOwned < entry.Amount)
                .ToArray();

            Gatherable = RequiredItems
                .Where(entry => !entry.Item.IsCrystal && entry.Item.IsGatherable && entry.Item.FishingSpot == null && !entry.Item.IsCraftable)
                .ToArray();

            Fishable = RequiredItems
                .Where(entry => !entry.Item.IsCrystal && !entry.Item.IsGatherable && entry.Item.FishingSpot != null && !entry.Item.IsCraftable)
                .ToArray();

            OtherSources = RequiredItems
                .Where(entry => !entry.Item.IsCrystal && !entry.Item.IsGatherable && entry.Item.FishingSpot == null && !entry.Item.IsCraftable)
                .ToArray();

            Craftable = RequiredItems
                .Where(entry => !entry.Item.IsCrystal && !entry.Item.IsGatherable && entry.Item.FishingSpot == null && entry.Item.IsCraftable)
                .ToArray();

            // add items required for leves last
            var leveRequiredItems = new List<RequiredItem>();
            foreach (var leve in acceptedCraftLeves)
            {
                if (leve.RequiredItems == null)
                    continue;

                foreach (var entry in leve.RequiredItems)
                {
                    leveRequiredItems.Add(entry);
                }
            }
            LeveRequiredItems = leveRequiredItems.ToArray();
        }
    }

    public static int CalculateDepth(RequiredItem root, int depth)
    {
        var result = depth + 1;

        if (root.Item.IsCraftable)
        {
            foreach (var node in root.Item.Ingredients!)
                result = Math.Max(result, CalculateDepth(node, depth + 1));
        }

        return result;
    }

    private List<RequiredItem> GetItemsAtDepth(RequiredItem root, int depth, int neededDepth, uint parentAmount)
    {
        var result = new List<RequiredItem>();

        if (root.Item.IsCraftable && root.Item.QuantityOwned < root.Amount)
        {
            foreach (var ingredient in root.Item.Ingredients!)
            {
                if (depth == neededDepth)
                {
                    result.Add(new RequiredItem(ingredient.Item, ingredient.Amount * parentAmount));
                }
                else if (ingredient.Item.IsCraftable)
                {
                    result.AddRange(GetItemsAtDepth(ingredient, depth + 1, neededDepth, (uint)Math.Ceiling(ingredient.Amount * parentAmount / (double)(ingredient.Item.Recipe?.AmountResult ?? 1))));
                }
            }
        }

        return result;
    }

    private void ProcessIngredients(RequiredItem req, HashSet<uint> visited, List<RequiredItem> sorted)
    {
        if (!visited.Contains(req.Item.ItemId))
        {
            visited.Add(req.Item.ItemId);

            if (req.Item.Ingredients != null)
            {
                foreach (var dep in req.Item.Ingredients)
                    ProcessIngredients(dep, visited, sorted);
            }

            sorted.Add(req);
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
            RefreshQuantities();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.SetTooltip("Refresh quantities");
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
                ImGui.Text("Needed Crystals:");
                foreach (var entry in Crystals)
                {
                    DrawItem(entry.Item, entry.Amount, $"Item{i}");
                    i++;
                }
            }

            if (Gatherable.Any())
            {
                ImGui.Text("Gather:");
                foreach (var entry in Gatherable)
                {
                    DrawItem(entry.Item, entry.Amount, $"Item{i}");
                    i++;
                }
            }

            if (Fishable.Any())
            {
                ImGui.Text("Fish:");
                foreach (var entry in Fishable)
                {
                    DrawItem(entry.Item, entry.Amount, $"Item{i}");
                    i++;
                }
            }

            if (OtherSources.Any())
            {
                ImGui.Text("Other:");
                foreach (var entry in OtherSources)
                {
                    DrawItem(entry.Item, entry.Amount, $"Item{i}");
                    i++;
                }
            }

            if (Craftable.Any())
            {
                ImGui.Text("Craft:");
                foreach (var entry in Craftable)
                {
                    DrawItem(entry.Item, entry.Amount, $"Item{i}");
                    i++;
                }
            }

            ImGui.Separator();

            foreach (var entry in LeveRequiredItems)
            {
                DrawItem(entry.Item, entry.Amount, $"Item{i}");
                i++;
            }
        }
    }
}
