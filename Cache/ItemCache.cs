using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class ItemCache
{
    public static readonly Dictionary<uint, CachedItem> Cache = new();

    public static CachedItem Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }

    public static CachedItem Get(Item item)
    {
        if (!Cache.TryGetValue(item.RowId, out var cachedItem))
            Cache.Add(item.RowId, cachedItem = new(item));

        return cachedItem;
    }
}

public record CachedItem
{
    public CachedItem(uint itemId)
    {
        ItemId = itemId;
    }

    public CachedItem(Item item)
    {
        ItemId = item.RowId;
        Item = item;
    }

    private Item? item { get; set; } = null;
    private string itemName { get; set; } = "";
    private Recipe? recipe { get; set; } = null;
    private bool? isGatherable { get; set; } = null;
    private FishingSpot? fishingSpot { get; set; } = null;
    private RequiredItem[]? ingredients { get; set; } = null;
    private uint? quantityOwned { get; set; } = null;
    private DateTime quantityOwnedLastUpdate { get; set; }

    public uint ItemId { get; init; }

    public Item? Item
    {
        get => item ??= Service.Data.GetExcelSheet<Item>()?.GetRow(ItemId);
        private set => item = value;
    }

    public string ItemName
    {
        get
        {
            if (string.IsNullOrEmpty(itemName))
            {
                unsafe
                {
                    var ptr = (nint)Framework.Instance()->GetUiModule()->GetRaptureTextModule()->FormatAddonText2(2021, (int)ItemId, 0);
                    if (ptr != 0)
                        itemName = MemoryHelper.ReadSeStringNullTerminated(ptr).ToString();
                }
            }

            return !string.IsNullOrEmpty(itemName)
                ? itemName
                : Item?.Name.ClearString() ?? "";
        }
    }

    public int Icon
        => Item?.Icon ?? 0;

    public Recipe? Recipe
        => recipe ??= Service.Data.GetExcelSheet<Recipe>()?.FirstOrDefault(recipe => recipe.ItemResult.Value?.RowId == ItemId);

    public bool IsCrystal
        => Item?.ItemUICategory.Row == 59;

    public bool IsGatherable
        => (isGatherable ??= Service.Data.GetExcelSheet<GatheringItem>()?.Any(row => row.Item == ItemId)) ?? false;

    public bool IsCraftable
        => Recipe != null;

    public FishingSpot? FishingSpot
        => fishingSpot ??= Service.Data.GetExcelSheet<FishingSpot>()?.FirstOrDefault(row => row.Item.Any(i => i.Row == ItemId));

    public RequiredItem[] Ingredients
    {
        get
        {
            if (ingredients != null)
                return ingredients;

            if (!IsCraftable)
                return ingredients ??= Array.Empty<RequiredItem>();

            var list = new List<RequiredItem>();

            foreach (var ingredient in Recipe!.UnkData5)
            {
                if (ingredient.ItemIngredient == 0 || ingredient.AmountIngredient == 0)
                    continue;

                list.Add(new(ItemCache.Get((uint)ingredient.ItemIngredient), ingredient.AmountIngredient));
            }

            return ingredients = list.ToArray();
        }
    }

    public uint QuantityOwned
        => quantityOwned ?? UpdateQuantityOwned();

    public unsafe uint UpdateQuantityOwned()
    {
        var inventoryManager = InventoryManager.Instance();

        quantityOwned = 0
            + (uint)inventoryManager->GetInventoryItemCount(ItemId)
            + (uint)inventoryManager->GetInventoryItemCount(ItemId, true);

        return (uint)quantityOwned;
    }

    public bool HasAllIngredients
        => Ingredients != null && Ingredients.All(ingredient => ingredient.Item.QuantityOwned > ingredient.Amount);

    public ItemQueueCategory QueueCategory
    {
        get
        {
#pragma warning disable IDE0046
            if (IsCrystal)
                return ItemQueueCategory.Crystals;
            
            if (IsGatherable)
                return ItemQueueCategory.Gatherable;

            if (FishingSpot != null)
                return ItemQueueCategory.Fishable;

            if (IsCraftable)
                return ItemQueueCategory.Craftable;

            return ItemQueueCategory.OtherSources;
#pragma warning restore IDE0046
        }
    }
}

public enum ItemQueueCategory
{
    None,
    Crystals,
    Gatherable,
    Fishable,
    OtherSources,
    Craftable,
}
