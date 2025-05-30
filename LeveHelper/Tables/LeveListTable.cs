using System.Linq;
using AutoCtor;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Graphics;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using LeveHelper.Tables.Columns;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables;

[RegisterTransient, AutoConstruct]
public partial class LeveListTable : Table<Leve>
{
    public static Color ComboBorder { get; } = hex(0x836627);

    private static readonly uint[] ExcludedLeves = {
        502, 508, 514, 519, 525,
        531, 542, 544, 552, 554,
        562, 564, 582, 597, 822,
        827, 832, 871, 872, 877,
    };

    private readonly ExcelService _excelService;
    private readonly LeveService _leveService;
    private readonly IClientState _clientState;
    private readonly RowIdColumn rowIdColumn;
    private readonly LevelColumn levelColumn;
    private readonly StatusColumn statusColumn;
    private readonly TypeColumn typeColumn;
    private readonly NameColumn nameColumn;
    private readonly LevemeteColumn levemeteColumn;

    [AutoPostConstruct]
    private void Initialize()
    {
        Id = "LeveListTable";

        Columns = [
            rowIdColumn,
            levelColumn,
            statusColumn,
            typeColumn,
            nameColumn,
            levemeteColumn
        ];

        foreach (var col in Columns.OfType<IConnectedColumn<LeveListTable>>())
        {
            col.SetTable(this);
        }

        Flags |= ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable;

        _clientState.Login += OnLogin;
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    private void OnLogin()
    {
        RowsLoaded = false; // trigger reload
    }

    public override unsafe void LoadRows()
    {
        var town = PlayerState.Instance()->StartTown;
        Rows = _excelService.GetSheet<Leve>()
            .Where(row => row.LeveClient.RowId != 0 && !ExcludedLeves.Contains(row.RowId) && (!_leveService.IsTownLocked(row.RowId) || row.Town.RowId == town))
            .ToList();
    }

    public unsafe int GetNumTotalLeves()
    {
        return (_filteredRows ?? Rows).Count;
    }

    public int GetNeededAllowances()
    {
        return (_filteredRows ?? Rows).Where(leve => !_leveService.IsComplete(leve.RowId)).Sum(row => row.AllowanceCost);
    }
}
