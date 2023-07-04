namespace LeveHelper;

public class FishingSpot : Lumina.Excel.GeneratedSheets.FishingSpot
{
    private uint? _icon { get; set; } = null;

    public uint Icon
        => _icon ??= !Rare ? 60465u : 60466u;
}
