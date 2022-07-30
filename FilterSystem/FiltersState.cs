using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using ImGuiNET;

namespace LeveHelper;

public class FiltersState
{
    public readonly LeveRecord[] AllLeves = null!;
    public readonly uint[] AllLocations = null!;

    public IEnumerable<LeveRecord> Leves = null!;
    public LeveRecord[] LevesArray = null!;

    public short SortColumnIndex;
    public ImGuiSortDirection SortDirection;

    public int NumCompletedLeves = 0;
    public int NumTotalLeves = 0;
    public int NeededAllowances = 0;

    // data from a certain Levequests Checklist spreadsheet
    // seem to be unused test levequests
    public readonly uint[] ExcludedLeves = {
        502, 508, 514, 519, 525,
        531, 542, 544, 552, 554,
        562, 564, 582, 597, 822,
        827, 832, 871, 872, 877,
    };

    public FiltersState()
    {
        AllLeves = Service.Data.GetExcelSheet<Leve>()!
            .Where(row => row.LeveClient.Row != 0 && !ExcludedLeves.Contains(row.RowId))
            .Select(row => new LeveRecord(row))
            .ToArray();

        AllLocations = AllLeves
            .Select(row => row.leve.PlaceNameStartZone.Value)
            .Where(item => item != null)
            .Cast<PlaceName>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First().RowId)
            .ToArray();
    }
}
