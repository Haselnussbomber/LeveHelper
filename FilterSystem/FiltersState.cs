using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using ImGuiNET;

namespace LeveHelper;

public class FiltersState
{
    public readonly LeveRecord[] AllLeves = null!;
    public readonly uint[] AllLocations = null!;

    public IEnumerable<LeveRecord> leves = null!;
    public LeveRecord[] levesArray = null!;
    public Dictionary<uint, string> locations = null!;
    public Dictionary<uint, string> classes = null!;
    public Dictionary<uint, string> levemetes = null!;

    public short sortColumnIndex { get; set; }
    public ImGuiSortDirection sortDirection { get; set; }

    public int numCompletedLeves = 0;
    public int numTotalLeves = 0;
    public int neededAllowances = 0;

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
            .Select(row => row.value.PlaceNameStartZone.Value)
            .Where(item => item != null)
            .Cast<PlaceName>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First().RowId)
            .ToArray();
    }
}
