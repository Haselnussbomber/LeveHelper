using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Extensions;

public static class GameObjectExtensions
{
    public static SeString? GetMapLink(this GameObject obj)
    {
        var territoryId = Service.ClientState.TerritoryType;
        if (territoryId == 0) return null;

        var territoryType = Service.Data.GetExcelSheet<TerritoryType>()?.GetRow(territoryId);
        if (territoryType == null) return null;

        var mapLinkPayload = new MapLinkPayload(
            territoryId,
            territoryType.Map.Row,
            ToCoord(obj.Position.X, territoryType.Map.Value?.SizeFactor ?? 100) + territoryType.Map.Value?.OffsetX ?? 0,
            ToCoord(obj.Position.Z, territoryType.Map.Value?.SizeFactor ?? 100) + territoryType.Map.Value?.OffsetY ?? 0,
            0.05f
        );

        var sb = new SeStringBuilder();

        sb.Add(mapLinkPayload);
        sb.Append(new SeString(new List<Payload>(SeString.TextArrowPayloads)));
        sb.AddText(" ");
        sb.AddUiForeground(1);
        sb.AddText(obj.Name.TextValue);
        sb.AddUiForegroundOff();
        sb.Add(RawPayload.LinkTerminator);

        return sb.Build();
    }

    /// <see cref="https://github.com/xivapi/ffxiv-datamining/blob/master/docs/MapCoordinates.md"/>
    private static float ToCoord(float value, ushort scale)
    {
        var tileScale = 2048f / 41f;
        return value / tileScale + 2048f / (scale / 100f) / tileScale / 2 + 1;
    }
}
