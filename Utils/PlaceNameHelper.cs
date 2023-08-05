using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public class PlaceNameHelper
{
    private static bool IsSubscribed;

    public static uint PlaceNameId { get; private set; }

    public static void Connect()
    {
        if (IsSubscribed)
            return;

        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;

        IsSubscribed = true;

        Update();
    }

    public static void Disconnect()
    {
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
        IsSubscribed = false;
    }

    private static void ClientState_TerritoryChanged(object? sender, ushort e)
    {
        Update();
    }

    private static void Update()
    {
        var curTerritory = Service.DataManager.GetExcelSheet<TerritoryType>()?.GetRow(Service.ClientState.TerritoryType);
        PlaceNameId = curTerritory?.PlaceName?.Row ?? 0;
    }
}
