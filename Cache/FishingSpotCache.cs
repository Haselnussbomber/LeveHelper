using System.Collections.Generic;
using System.Linq;
using Lumina.Excel;
using Item = Lumina.Excel.GeneratedSheets.Item;

namespace LeveHelper;

public static class FishingSpotCache
{
    public static readonly Dictionary<uint, FishingSpot[]?> CacheByItemId = new();

    public static FishingSpot[]? FindByItemId(uint id)
    {
        if (!CacheByItemId.TryGetValue(id, out var cachedFishingSpot))
        {
            cachedFishingSpot = GetSheet<FishingSpot>()
                .Where(row => row.TerritoryType.Row != 0 && row.Item.Any<LazyRow<Item>>(i => i.Row == id))
                .Select(row => GetRow<FishingSpot>(row.RowId)!)
                .ToArray();

            CacheByItemId.Add(id, cachedFishingSpot);
        }

        return cachedFishingSpot;
    }
}
