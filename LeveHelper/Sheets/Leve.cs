using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Sheets;

public class Leve : HaselCommon.Sheets.Leve
{
    private RequiredItem[]? _requiredItems = null;

    public RequiredItem[] RequiredItems
    {
        get
        {
            if (_requiredItems != null)
                return _requiredItems ??= Array.Empty<RequiredItem>();

            if (IsCraftLeve)
            {
                var craftLeve = GetRow<CraftLeve>((uint)DataId);
                if (craftLeve != null)
                {
                    return _requiredItems = craftLeve.UnkData3
                        .Where(item => item.Item != 0 && item.ItemCount != 0)
                        .Aggregate(
                            new Dictionary<int, RequiredItem>(),
                            (dict, entry) =>
                            {
                                if (!dict.TryGetValue(entry.Item, out var reqItem))
                                {
                                    reqItem = new RequiredItem(GetRow<Item>((uint)entry.Item)!, 0);
                                    dict.Add(entry.Item, reqItem);
                                }

                                reqItem.Amount += entry.ItemCount;

                                return dict;
                            })
                        .Values
                        .ToArray();
                }
            }

            return _requiredItems ??= Array.Empty<RequiredItem>();
        }
    }
}
