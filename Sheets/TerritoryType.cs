namespace LeveHelper.Sheets;

public class TerritoryType : Lumina.Excel.GeneratedSheets.TerritoryType
{
    private string? _placeName { get; set; } = null;

    public new string PlaceName
        => _placeName ??= base.PlaceName.Value?.Name.ClearString() ?? "";
}
