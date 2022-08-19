using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class LevelExtension
{
    public static void OpenMapLocation(this Level level)
    {
        var map = level.Map.Value;
        var terr = map!.TerritoryType.Value;

        Service.GameGui.OpenMapWithMapLink(
            new MapLinkPayload(
                terr!.RowId,
                map.RowId,
                (int)(level!.X * 1_000f),
                (int)(level.Z * 1_000f)
            )
        );
    }
}
