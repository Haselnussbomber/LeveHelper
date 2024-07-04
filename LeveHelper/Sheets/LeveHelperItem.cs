using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace LeveHelper.Sheets;

public class LeveHelperItem : HaselCommon.Sheets.ExtendedItem
{
    private RequiredItem[]? _ingredients { get; set; } = null;
    private uint? _quantityOwned { get; set; } = null;
    private DateTime _quantityOwnedLastUpdate { get; set; }

    public RequiredItem[] Ingredients
    {
        get
        {
            if (_ingredients != null)
                return _ingredients;

            if (!IsCraftable)
                return _ingredients ??= Array.Empty<RequiredItem>();

            var list = new List<RequiredItem>();
            var recipe = Recipes.First();

            foreach (var ingredient in recipe.UnkData5)
            {
                if (ingredient.ItemIngredient == 0 || ingredient.AmountIngredient == 0)
                    continue;

                list.Add(new(GetRow<LeveHelperItem>((uint)ingredient.ItemIngredient)!, ingredient.AmountIngredient));
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
            if (IsCrystal)
                return ItemQueueCategory.Crystals;

            if (IsGatherable || IsFish)
                return ItemQueueCategory.Gatherable;

            if (IsCraftable)
                return ItemQueueCategory.Craftable;

            return ItemQueueCategory.OtherSources;
        }
    }
}
