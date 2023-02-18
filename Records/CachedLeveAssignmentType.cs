using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record CachedLeveAssignmentType
{
    public CachedLeveAssignmentType(uint rowId)
    {
        RowId = rowId;
    }

    public CachedLeveAssignmentType(LeveAssignmentType leveAssignmentType)
    {
        RowId = leveAssignmentType.RowId;
        this.leveAssignmentType = leveAssignmentType;
    }

    private LeveAssignmentType? leveAssignmentType { get; set; } = null;
    private string? name { get; set; } = null;
    private int? icon { get; set; } = null;

    public uint RowId { get; }

    public LeveAssignmentType? LeveAssignmentType
        => leveAssignmentType ??= Service.Data.GetExcelSheet<LeveAssignmentType>()?.GetRow(RowId);

    public string Name
        => name ??= LeveAssignmentType?.Name.ClearString() ?? "";

    public int Icon
        => icon ??= LeveAssignmentType?.Icon ?? 0;
}
