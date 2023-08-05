using System.Collections.Generic;
using System.Linq;
using GatheringItem = Lumina.Excel.GeneratedSheets.GatheringItem;

namespace LeveHelper;

public static class GatheringPointCache
{
    public static readonly Dictionary<uint, GatheringPoint> Cache = new();
    public static readonly Dictionary<uint, GatheringPoint[]> CacheByItemId = new();

    public static GatheringPoint[]? FindByItemId(uint id)
        => CacheByItemId.TryGetValue(id, out var cachedGatheringPoint) ? cachedGatheringPoint : null;

    public static void Load()
    {
        var gatheringPointSheet = Service.DataManager.GetExcelSheet<GatheringPoint>();
        if (gatheringPointSheet == null)
            return;

        foreach (var point in gatheringPointSheet)
        {
            if (point.GatheringPointBase.Value == null || point.TerritoryType.Row == 1)
                continue;

            foreach (var itemRowId in point.GatheringPointBase.Value.Item)
            {
                if (itemRowId == 0)
                    continue;

                var itemRow = Service.DataManager.GetExcelSheet<GatheringItem>()?.GetRow((uint)itemRowId);
                if (itemRow == null)
                    continue;

                if (CacheByItemId.TryGetValue((uint)itemRow.Item, out var cachedGatheringPoints))
                {
                    var list = cachedGatheringPoints.ToList();
                    list.Add(Service.DataManager.GetExcelSheet<GatheringPoint>()!.GetRow(point.RowId)!);
                    CacheByItemId[(uint)itemRow.Item] = list.ToArray();
                }
                else
                {
                    CacheByItemId.Add((uint)itemRow.Item, new[] { Service.DataManager.GetExcelSheet<GatheringPoint>()!.GetRow(point.RowId)! });
                }
            }
        }
    }
}
