using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

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
