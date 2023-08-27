using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace LeveHelper.Sheets;

public class Item : HaselCommon.Sheets.Item
{
    private uint? _classJobIcon { get; set; } = null;
    private uint? _resultAmount { get; set; } = null;
    private RequiredItem[]? _ingredients { get; set; } = null;
    private uint? _quantityOwned { get; set; } = null;
    private DateTime _quantityOwnedLastUpdate { get; set; }

    public uint ResultAmount
        => _resultAmount ??= Recipe?.AmountResult ?? 1;

    public uint? ClassJobIcon
    {
        get
        {
            if (_classJobIcon != null)
                return _classJobIcon;

            if (IsCraftable)
            {
                _classJobIcon ??= 62008 + Recipe?.CraftType.Row;
            }
            else if (IsGatherable)
            {
                _classJobIcon ??= GatheringPoints.First().Icon;
            }
            else if (IsFish)
            {
                _classJobIcon ??= FishingSpots.First().Icon;
            }

            return _classJobIcon;
        }
    }

    public RequiredItem[] Ingredients
    {
        get
        {
            if (_ingredients != null)
                return _ingredients;

            if (!IsCraftable)
                return _ingredients ??= Array.Empty<RequiredItem>();

            var list = new List<RequiredItem>();

            foreach (var ingredient in Recipe!.UnkData5)
            {
                if (ingredient.ItemIngredient == 0 || ingredient.AmountIngredient == 0)
                    continue;

                list.Add(new(GetRow<Item>((uint)ingredient.ItemIngredient)!, ingredient.AmountIngredient));
            }

            return _ingredients = list.ToArray();
        }
    }

    public uint QuantityOwned
        => _quantityOwned ?? UpdateQuantityOwned();

    public unsafe uint UpdateQuantityOwned()
    {
        var inventoryManager = InventoryManager.Instance();

        _quantityOwned = 0
            + (uint)inventoryManager->GetInventoryItemCount(RowId)
            + (uint)inventoryManager->GetInventoryItemCount(RowId, true);

        return (uint)_quantityOwned;
    }

    public bool HasAllIngredients
        => Ingredients.All(ingredient => ingredient.Item.QuantityOwned > ingredient.Amount);

    public ItemQueueCategory QueueCategory
    {
        get
        {
#pragma warning disable IDE0046
            if (IsCrystal)
                return ItemQueueCategory.Crystals;

            if (IsGatherable || IsFish)
                return ItemQueueCategory.Gatherable;

            if (IsCraftable)
                return ItemQueueCategory.Craftable;

            return ItemQueueCategory.OtherSources;
#pragma warning restore IDE0046
        }
    }
}
