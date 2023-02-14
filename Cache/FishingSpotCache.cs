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
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }

    public static CachedFishingSpot[]? FindByItemId(uint id)
    {
        if (!CacheByItemId.TryGetValue(id, out var item))
        {
            item = Service.Data.GetExcelSheet<FishingSpot>()?
                .Where(row => row.TerritoryType.Row != 0 && row.Item.Any(i => i.Row == id))
                .Select(row => Get(row.RowId))
                .ToArray();

            CacheByItemId.Add(id, item);
        }

        return item;
    }
}

public record CachedFishingSpot
{
    public CachedFishingSpot(uint rowId)
    {
        RowId = rowId;
    }

    private FishingSpot? fishingSpot { get; set; } = null;
    private TerritoryType? territoryType { get; set; } = null;
    private uint? territoryTypeId { get; set; } = null;
    private uint? placeNameId { get; set; } = null;
    private string? placeName { get; set; } = null;
    private uint? icon { get; set; } = null;

    public uint RowId { get; init; }

    public FishingSpot? FishingSpot
        => fishingSpot ??= Service.Data.GetExcelSheet<FishingSpot>()?.GetRow(RowId);

    public TerritoryType? TerritoryType
        => territoryType ??= FishingSpot?.TerritoryType.Value;

    public uint TerritoryTypeId
        => territoryTypeId ??= FishingSpot?.TerritoryType.Row ?? 0;

    public uint PlaceNameId
        => placeNameId ??= FishingSpot?.TerritoryType.Value?.PlaceName.Row ?? 0;

    public string PlaceName
        => placeName ??= FishingSpot?.TerritoryType.Value?.PlaceName.Value?.Name.ClearString() ?? "";

    public uint Icon
        => icon ??= FishingSpot?.Rare == false ? 60465u : 60466u;
}
