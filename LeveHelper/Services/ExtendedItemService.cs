using System.Collections.Generic;
using System.Linq;
using AutoCtor;
using HaselCommon.Services;
using HaselCommon.Utils;
using LeveHelper.Caches;
using LeveHelper.Enums;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public partial class ExtendedItemService : ItemService
{
    private readonly ItemQuantityCache _itemQuantityCache = new();
    private readonly Dictionary<uint, ItemQueueCategory> _itemQueueCategoryCache = [];

    public void InvalidateQuantity(uint itemId)
        => _itemQuantityCache.Remove(itemId);

    public uint GetQuantity(uint itemId)
        => _itemQuantityCache.GetValue(itemId);

    public bool HasAllIngredients(uint itemId)
        => GetIngredients(itemId).All(ingredient => GetQuantity(ingredient.Item) > ingredient.Amount);

    public ItemQueueCategory GetQueueCategory(ItemHandle item)
    {
        if (_itemQueueCategoryCache.TryGetValue(item, out var category))
            return category;

        if (!item.TryGetItem(out _))
        {
            category = ItemQueueCategory.None;
        }
        else if (item.IsCrystal)
        {
            category = ItemQueueCategory.Crystals;
        }
        else if (item.IsGatherable || item.IsFish || item.IsSpearfish)
        {
            category = ItemQueueCategory.Gatherable;
        }
        else if (item.IsCraftable)
        {
            category = ItemQueueCategory.Craftable;
        }
        else
        {
            category = ItemQueueCategory.OtherSources;
        }

        _itemQueueCategoryCache.Add(item, category);
        return category;
    }
}
