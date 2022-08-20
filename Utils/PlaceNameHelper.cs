using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public class PlaceNameHelper
{
    private static bool subscribed;

    public static uint PlaceNameId { get; private set; }

    public static void Connect()
    {
        if (subscribed) return;
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        subscribed = true;

        Update();
    }

    public static void Disconnect()
    {
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
        subscribed = false;
    }

    private static void ClientState_TerritoryChanged(object? sender, ushort e)
    {
        Update();
    }

    private static void Update()
    {
        var curTerritory = Service.Data.GetExcelSheet<TerritoryType>()?.GetRow(Service.ClientState.TerritoryType);
        PlaceNameId = curTerritory?.PlaceName?.Row ?? 0;
    }
}
