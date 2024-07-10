using System.Collections.Generic;
using System.Linq;
using HaselCommon.Caching;
using HaselCommon.Services;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Caches;

public class LeveRequiredItemsCache(ExcelService ExcelService, LeveService LeveService) : MemoryCache<uint, RequiredItem[]>
{
    public override RequiredItem[]? CreateEntry(uint leveRowId)
    {
        var leve = ExcelService.GetRow<Leve>(leveRowId);
        if (leve == null)
            return null;

        if (!LeveService.IsCraftLeve(leve))
            return null;

        var craftLeve = ExcelService.GetRow<CraftLeve>((uint)leve.DataId);
        if (craftLeve == null)
            return null;

        return craftLeve.UnkData3
            .Where(item => item.Item != 0 && item.ItemCount != 0)
            .Aggregate(
                new Dictionary<int, RequiredItem>(),
                (dict, entry) =>
                {
                    if (!dict.TryGetValue(entry.Item, out var reqItem))
                    {
                        reqItem = new RequiredItem(ExcelService.GetRow<Item>((uint)entry.Item)!, 0);
                        dict.Add(entry.Item, reqItem);
                    }

                    reqItem.Amount += entry.ItemCount;

                    return dict;
                })
            .Values
            .ToArray();
    }
}
