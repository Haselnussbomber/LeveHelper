using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using LeveHelper.Caches;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Services;

public class ExtendedItemService : ItemService
{
    private readonly ExcelService ExcelService;
    private readonly IGameInventory GameInventory;
    private readonly ItemIngredientsCache ItemIngredientsCache;
    private readonly ItemQuantityCache ItemQuantityCache = new();

    public ExtendedItemService(ExcelService excelService, TextService textService, IGameInventory gameInventory) : base(excelService, textService)
    {
        ExcelService = excelService;
        GameInventory = gameInventory;
        ItemIngredientsCache = new ItemIngredientsCache(excelService, this);

        GameInventory.InventoryChangedRaw += GameInventory_InventoryChangedRaw;
    }

    public void Dispose()
    {
        GameInventory.InventoryChangedRaw -= GameInventory_InventoryChangedRaw;
    }

    private void GameInventory_InventoryChangedRaw(IReadOnlyCollection<InventoryEventArgs> events)
    {
        foreach (var evt in events)
            InvalidateQuantity(evt.Item.ItemId);
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

        if (IsGatherable(item) || IsFish(item))
            return ItemQueueCategory.Gatherable;

        if (IsCraftable(item))
            return ItemQueueCategory.Craftable;

        return ItemQueueCategory.OtherSources;
    }
}
