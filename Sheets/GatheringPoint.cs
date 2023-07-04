namespace LeveHelper;

public class GatheringPoint : Lumina.Excel.GeneratedSheets.GatheringPoint
{
    private uint? _icon { get; set; } = null;

    public uint Icon
    {
        get
        {
            if (_icon != null)
                return _icon ??= 0;

            var gatheringType = GatheringPointBase.Value?.GatheringType.Value;
            if (gatheringType == null)
                return _icon ??= 0;

            var rare = !Service.GameFunctions.IsGatheringPointRare(Type);
            return _icon ??= rare ? (uint)gatheringType.IconMain : (uint)gatheringType.IconOff;
        }
    }
}
