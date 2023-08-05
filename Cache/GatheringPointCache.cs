using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class GatheringPointCache
{
    public static readonly Dictionary<uint, GatheringPoint> Cache = new();
    public static readonly Dictionary<uint, GatheringPoint[]> CacheByItemId = new();

    public static GatheringPoint[]? FindByItemId(uint id)
        => CacheByItemId.TryGetValue(id, out var cachedGatheringPoint) ? cachedGatheringPoint : null;

    public static void Load()
    {
        foreach (var point in GetSheet<GatheringPoint>())
        {
            if (point.GatheringPointBase.Value == null || point.TerritoryType.Row == 1)
                continue;

            foreach (var itemRowId in point.GatheringPointBase.Value.Item)
            {
                if (itemRowId == 0)
                    continue;

                var itemRow = GetRow<GatheringItem>((uint)itemRowId);
                if (itemRow == null)
                    continue;

                if (CacheByItemId.TryGetValue((uint)itemRow.Item, out var cachedGatheringPoints))
                {
                    var list = cachedGatheringPoints.ToList();
                    list.Add(GetRow<GatheringPoint>(point.RowId)!);
                    CacheByItemId[(uint)itemRow.Item] = list.ToArray();
                }
                else
                {
                    CacheByItemId.Add((uint)itemRow.Item, new[] { GetRow<GatheringPoint>(point.RowId)! });
                }
            }
        }
    }
}
