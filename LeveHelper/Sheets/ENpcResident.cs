using Dalamud.Utility;

namespace LeveHelper.Sheets;

public class ENpcResident : Lumina.Excel.GeneratedSheets.ENpcResident
{
    private string? _singular { get; set; } = null;

    public new string Singular
        => _singular ??= base.Singular.ToDalamudString().ToString();
}
