namespace LeveHelper.Sheets;

public class ENpcResident : Lumina.Excel.GeneratedSheets.ENpcResident
{
    private string? _name { get; set; } = null;

    public string Name
        => _name ??= GetENpcResidentName(RowId);
}
