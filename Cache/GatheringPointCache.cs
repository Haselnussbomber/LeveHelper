using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class GatheringPointCache
{
    public static readonly Dictionary<uint, CachedGatheringPoint> Cache = new();
    public static readonly Dictionary<uint, CachedGatheringPoint[]> CacheByItemId = new();

    public static CachedGatheringPoint Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var cachedGatheringPoint))
            Cache.Add(id, cachedGatheringPoint = new(id));

        return cachedGatheringPoint;
    }

    public static CachedGatheringPoint[]? FindByItemId(uint id)
        => CacheByItemId.TryGetValue(id, out var cachedGatheringPoint) ? cachedGatheringPoint : null;

    public static void Load()
    {
        var gatheringPointSheet = Service.Data.GetExcelSheet<GatheringPoint>();
        if (gatheringPointSheet == null)
            return;

        foreach (var point in gatheringPointSheet)
        {
            if (point.GatheringPointBase.Value == null || point.TerritoryType.Row == 1)
                continue;

            foreach (var itemRowId in point.GatheringPointBase.Value.Item)
            {
                if (itemRowId == 0) break;

                var itemRow = Service.Data.GetExcelSheet<GatheringItem>()?.GetRow((uint)itemRowId);
                if (itemRow == null)
                    continue;

                if (CacheByItemId.TryGetValue((uint)itemRow.Item, out var cachedGatheringPoints))
                {
                    var list = cachedGatheringPoints.ToList();
                    list.Add(Get(point.RowId));
                    CacheByItemId[(uint)itemRow.Item] = list.ToArray();
                }
                else
                {
                    CacheByItemId.Add((uint)itemRow.Item, new[] { Get(point.RowId) });
                }
            }
        }
    }
}
