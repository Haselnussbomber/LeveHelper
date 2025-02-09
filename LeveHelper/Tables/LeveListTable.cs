using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Extensions.Collections;
using HaselCommon.Extensions.Strings;
using HaselCommon.Graphics;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Tables.LeveListTableColumns;
using Lumina.Excel.Sheets;
using static HaselCommon.Globals.ColorHelpers;

namespace LeveHelper.Tables;

[RegisterTransient]
public class LeveListTable : Table<Leve>
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

    public LeveListTable(
        RowIdColumn rowIdColumn,
        LevelColumn levelColumn,
        StatusColumn statusColumn,
        TypeColumn typeColumn,
        NameColumn nameColumn,
        LevemeteColumn levemeteColumn,

        // Services
        ExcelService excelService,
        LanguageProvider languageProvider,
        LeveService leveService,

        // Dalamud Services
        IClientState clientState) : base("LeveListTable", languageProvider)
    {
        _excelService = excelService;
        _leveService = leveService;
        _clientState = clientState;

        Columns = [
            rowIdColumn,
            levelColumn,
            statusColumn,
            typeColumn,
            nameColumn,
            levemeteColumn
        ];

        Columns.OfType<IConnectedColumn<LeveListTable>>().ForEach(col => col.SetTable(this));

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
            .Where(row => row.LeveClient.RowId != 0 && !ExcludedLeves.Contains(row.RowId) && (_leveService.IsTownLocked(row) ? row.Town.RowId == town : true))
            .ToList();
    }

    public unsafe int GetNumTotalLeves()
    {
        return (_filteredRows ?? Rows).Count;
    }

    public int GetNeededAllowances()
    {
        return (_filteredRows ?? Rows).Where(leve => !_leveService.IsComplete(leve)).Sum(row => row.AllowanceCost);
    }
}
