using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

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
    private uint? icon { get; set; } = null;

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

    public uint Icon
    {
        get
        {
            if (icon != null || GatheringPoint == null)
                return icon ??= 0;

            var gatheringType = GatheringPoint.GatheringPointBase.Value?.GatheringType.Value;
            if (gatheringType == null)
                return icon ??= 0;

            var rare = !Service.GameFunctions.IsGatheringPointRare(GatheringPoint.Type);
            return icon ??= rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
        }
    }
}
