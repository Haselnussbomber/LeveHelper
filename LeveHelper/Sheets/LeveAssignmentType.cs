using Dalamud.Utility;

namespace LeveHelper.Sheets;

public class LeveAssignmentType : Lumina.Excel.GeneratedSheets.LeveAssignmentType
{
    private string? _name { get; set; } = null;

    public string StringName
        => _name ??= base.Name.ToDalamudString().ToString() ?? "";
}
