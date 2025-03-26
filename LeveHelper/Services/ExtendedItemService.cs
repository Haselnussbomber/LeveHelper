using System.Collections.Generic;
using System.Linq;
using AutoCtor;
using HaselCommon.Services;
using LeveHelper.Caches;
using LeveHelper.Enums;
using Lumina.Excel.Sheets;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public partial class ExtendedItemService : ItemService
{
    private readonly ExcelService _excelService;
    private readonly ItemQuantityCache _itemQuantityCache = new();
    private readonly Dictionary<uint, ItemQueueCategory> _itemQueueCategoryCache = [];

    public void InvalidateQuantity(uint itemId)
        => _itemQuantityCache.Remove(itemId);

    public uint GetQuantity(uint itemId)
        => _itemQuantityCache.GetValue(itemId);

    public bool HasAllIngredients(uint itemId)
        => GetIngredients(itemId).All(ingredient => GetQuantity(ingredient.Item.RowId) > ingredient.Amount);

    public ItemQueueCategory GetQueueCategory(uint itemId)
    {
        if (_itemQueueCategoryCache.TryGetValue(itemId, out var category))
            return category;

        if (!_excelService.TryGetRow<Item>(itemId, out _))
        {
            category = ItemQueueCategory.None;
        }
        else if (IsCrystal(itemId))
        {
            category = ItemQueueCategory.Crystals;
        }
        else if (IsGatherable(itemId) || IsFish(itemId) || IsSpearfish(itemId))
        {
            category = ItemQueueCategory.Gatherable;
        }
        else if (IsCraftable(itemId))
        {
            category = ItemQueueCategory.Craftable;
        }
        else
        {
            category = ItemQueueCategory.OtherSources;
        }

        _itemQueueCategoryCache.Add(itemId, category);
        return category;
    }
}
