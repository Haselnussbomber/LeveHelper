using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class FishingSpotCache
{
    public static readonly Dictionary<uint, CachedFishingSpot> Cache = new();
    public static readonly Dictionary<uint, CachedFishingSpot[]?> CacheByItemId = new();

    public static CachedFishingSpot Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var cachedFishingSpot))
            Cache.Add(id, cachedFishingSpot = new(id));

        return cachedFishingSpot;
    }

    public static CachedFishingSpot[]? FindByItemId(uint id)
    {
        if (!CacheByItemId.TryGetValue(id, out var cachedFishingSpot))
        {
            cachedFishingSpot = Service.Data.GetExcelSheet<FishingSpot>()?
                .Where(row => row.TerritoryType.Row != 0 && row.Item.Any(i => i.Row == id))
                .Select(row => Get(row.RowId))
                .ToArray();

            CacheByItemId.Add(id, cachedFishingSpot);
        }

        return cachedFishingSpot;
    }
}
