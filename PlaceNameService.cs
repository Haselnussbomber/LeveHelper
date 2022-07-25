using System;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

internal class PlaceNameService : IDisposable
{
    internal uint PlaceNameId;

    public PlaceNameService()
    {
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        UpdatePlaceName();
    }

    private void ClientState_TerritoryChanged(object? sender, ushort e)
    {
        UpdatePlaceName();
    }

    private void UpdatePlaceName()
    {
        var curTerritory = Service.Data.GetExcelSheet<TerritoryType>()?.GetRow(Service.ClientState.TerritoryType);
        PlaceNameId = curTerritory?.PlaceName?.Row ?? 0;
    }

    void IDisposable.Dispose()
    {
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
    }
}
