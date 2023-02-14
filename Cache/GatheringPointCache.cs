using System.Collections.Generic;
using System.Linq;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class GatheringPointCache
{
    public static readonly Dictionary<uint, CachedGatheringPoint> Cache = new();
    public static readonly Dictionary<uint, CachedGatheringPoint[]> CacheByItemId = new();

    public static CachedGatheringPoint Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }

    public static CachedGatheringPoint[]? FindByItemId(uint id)
        => CacheByItemId.TryGetValue(id, out var item) ? item : null;

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
                PluginLog.Log($"{point.RowId}: {itemRowId}");

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

public record CachedGatheringPoint
{
    public CachedGatheringPoint(uint rowId)
    {
        RowId = rowId;
    }

    private GatheringPoint? gatheringPoint { get; set; } = null;
    private TerritoryType? territoryType { get; set; } = null;
    private uint? territoryTypeId { get; set; } = null;
    private uint? placeNameId { get; set; } = null;
    private string? placeName { get; set; } = null;

    public uint RowId { get; init; }

    public GatheringPoint? GatheringPoint
        => gatheringPoint ??= Service.Data.GetExcelSheet<GatheringPoint>()?.GetRow(RowId);

    public TerritoryType? TerritoryType
        => territoryType ??= GatheringPoint?.TerritoryType.Value;

    public uint TerritoryTypeId
        => territoryTypeId ??= GatheringPoint?.TerritoryType.Row ?? 0;

    public uint PlaceNameId
        => placeNameId ??= GatheringPoint?.TerritoryType.Value?.PlaceName.Row ?? 0;

    public string PlaceName
        => placeName ??= GatheringPoint?.TerritoryType.Value?.PlaceName.Value?.Name.ClearString() ?? "";
}
