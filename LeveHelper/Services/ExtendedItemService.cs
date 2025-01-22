using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using HaselCommon.Utils;
using LeveHelper.Caches;
using Lumina.Excel.Sheets;

namespace LeveHelper.Services;

[RegisterSingleton]
public class ExtendedItemService(IClientState clientState, ExcelService excelService, SeStringEvaluatorService seStringEvaluatorService, TextService textService) : ItemService(clientState, excelService, seStringEvaluatorService, textService)
{
    private readonly ItemQuantityCache _itemQuantityCache = new();
    private readonly Dictionary<uint, ItemQueueCategory> _itemQueueCategoryCache = [];

    public void InvalidateQuantity(ItemId itemId)
        => _itemQuantityCache.Remove(itemId);

    public uint GetQuantity(ItemId itemId)
        => _itemQuantityCache.GetValue(itemId);

    public bool HasAllIngredients(ItemId itemId)
        => GetIngredients(itemId).All(ingredient => GetQuantity(ingredient.Item.RowId) > ingredient.Amount);

    public ItemQueueCategory GetQueueCategory(ItemId itemId)
    {
        if (_itemQueueCategoryCache.TryGetValue(itemId, out var category))
            return category;

        if (!excelService.TryGetRow<Item>(itemId, out var item))
        {
            category = ItemQueueCategory.None;
        }
        else if (IsCrystal(item))
        {
            category = ItemQueueCategory.Crystals;
        }
        else if (IsGatherable(item) || IsFish(item) || IsSpearfish(item))
        {
            category = ItemQueueCategory.Gatherable;
        }
        else if (IsCraftable(item))
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
