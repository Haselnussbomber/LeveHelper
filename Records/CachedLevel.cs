using System.Numerics;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record CachedLevel
{
    public CachedLevel(uint rowId)
    {
        RowId = rowId;
    }

    private Level? level { get; set; } = null;
    private CachedTerritoryType? territoryType { get; set; } = null;
    private string? placeName { get; set; } = null;
    private Vector3? position { get; set; } = null;

    public uint RowId { get; }

    public Level? Level
        => level ??= Service.Data.GetExcelSheet<Level>()?.GetRow(RowId);

    public CachedTerritoryType? TerritoryType
        => territoryType ??= Level != null ? TerritoryTypeCache.Get(Level.Territory.Row) : null;

    public string PlaceName
        => placeName ??= TerritoryType?.PlaceName ?? "";

    public Vector3? Position
        => position ??= Level != null ? new(Level.X, Level.Y, Level.Z) : null;
}
