using System.Linq;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using LeveHelper.Caches;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Services;

public class ExtendedItemService : ItemService
{
    private readonly ExcelService ExcelService;
    private readonly ItemIngredientsCache ItemIngredientsCache;
    private readonly ItemQuantityCache ItemQuantityCache = new();

    public ExtendedItemService(IClientState clientState, ExcelService excelService, TextService textService) : base(clientState, excelService, textService)
    {
        ExcelService = excelService;
        ItemIngredientsCache = new ItemIngredientsCache(excelService, this);
    }

    public void InvalidateQuantity(Item item) => InvalidateQuantity(item.RowId);
    public void InvalidateQuantity(uint itemId)
        => ItemQuantityCache.Remove(itemId);

    public uint GetQuantity(Item item) => GetQuantity(item.RowId);
    public uint GetQuantity(uint itemId)
        => ItemQuantityCache.GetValue(itemId);

    public RequiredItem[] GetIngredients(Item item) => GetIngredients(item.RowId);
    public RequiredItem[] GetIngredients(uint itemId)
        => ItemIngredientsCache.GetValue(itemId) ?? [];

    public bool HasAllIngredients(Item item) => HasAllIngredients(item.RowId);
    public bool HasAllIngredients(uint itemId)
        => GetIngredients(itemId).All(ingredient => GetQuantity(ingredient.Item.RowId) > ingredient.Amount);

    public ItemQueueCategory GetQueueCategory(Item item) => GetQueueCategory(item.RowId);
    public ItemQueueCategory GetQueueCategory(uint itemId)
    {
        var item = ExcelService.GetRow<Item>(itemId);
        if (item == null)
            return ItemQueueCategory.None;

        if (IsCrystal(item))
            return ItemQueueCategory.Crystals;

        if (IsGatherable(item) || IsFish(item) || IsSpearfish(item))
            return ItemQueueCategory.Gatherable;

        if (IsCraftable(item))
            return ItemQueueCategory.Craftable;

        return ItemQueueCategory.OtherSources;
    }
}
