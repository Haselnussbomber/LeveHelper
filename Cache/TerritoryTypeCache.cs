using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class TerritoryTypeCache
{
    public static readonly Dictionary<uint, CachedTerritoryType> Cache = new();

    public static CachedTerritoryType Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }
}

public record CachedTerritoryType
{
    public CachedTerritoryType(uint rowId)
    {
        RowId = rowId;
    }

    private TerritoryType? territoryType { get; set; } = null;
    private string? placeName { get; set; } = null;

    public uint RowId { get; }

    public TerritoryType? TerritoryType
        => territoryType ??= Service.Data.GetExcelSheet<TerritoryType>()?.GetRow(RowId);

    public string PlaceName
        => placeName ??= TerritoryType?.PlaceName.Value?.Name.ClearString() ?? "";
}
