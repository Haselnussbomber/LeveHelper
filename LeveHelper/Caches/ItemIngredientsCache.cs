using System.Collections.Generic;
using System.Linq;
using HaselCommon.Caching;
using HaselCommon.Services;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Caches;

public class ItemIngredientsCache(ExcelService ExcelService, ItemService ItemService) : MemoryCache<uint, RequiredItem[]>
{
    public override RequiredItem[]? CreateEntry(uint itemId)
    {
        var recipes = ItemService.GetRecipes(itemId);

        if (recipes.Length == 0)
            return null;

        var list = new List<RequiredItem>();
        var recipe = recipes.First();

        foreach (var ingredient in recipe.UnkData5)
        {
            if (ingredient.ItemIngredient == 0 || ingredient.AmountIngredient == 0)
                continue;

            list.Add(new(ExcelService.GetRow<Item>((uint)ingredient.ItemIngredient)!, ingredient.AmountIngredient));
        }

        return [.. list];
    }
}
