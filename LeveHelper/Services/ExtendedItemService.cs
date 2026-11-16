using HaselCommon.Game.Enums;
using LeveHelper.Caches;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public partial class ExtendedItemService
{
    private readonly ItemService _itemService;

    private readonly ItemQuantityCache _itemQuantityCache = new();
    private readonly Dictionary<uint, ItemQueueCategory> _itemQueueCategoryCache = [];

    public void InvalidateQuantity(uint itemId)
        => _itemQuantityCache.Remove(itemId);

    public uint GetQuantity(uint itemId)
        => _itemQuantityCache.GetValue(itemId);

    public bool HasAllIngredients(uint itemId)
        => _itemService.GetIngredients(itemId).All(ingredient => GetQuantity(ingredient.Item) > ingredient.Amount);

    public bool IsCrystal(ItemHandle item)
        => _itemService.TryGetItem(item, out var itemRow) && itemRow.FilterGroup == (int)ItemFilterGroup.Crystal;

    public ItemQueueCategory GetQueueCategory(ItemHandle item)
    {
        if (_itemQueueCategoryCache.TryGetValue(item, out var category))
            return category;

        if (!_itemService.TryGetItem(item, out _))
        {
            category = ItemQueueCategory.None;
        }
        else if (IsCrystal(item))
        {
            category = ItemQueueCategory.Crystals;
        }
        else if (_itemService.IsGatherable(item) || _itemService.IsFish(item) || _itemService.IsSpearfish(item))
        {
            category = ItemQueueCategory.Gatherable;
        }
        else if (_itemService.IsCraftable(item))
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
