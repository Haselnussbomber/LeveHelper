using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using LeveHelper.Sheets;

namespace LeveHelper;

public class FiltersState
{
    public Leve[] AllLeves { get; private set; } = null!;

    public IEnumerable<Leve> Leves { get; set; } = null!;
    public Leve[] LevesArray { get; set; } = null!;

    public short SortColumnIndex { get; set; }
    public ImGuiSortDirection SortDirection { get; set; }

    public int NumCompletedLeves { get; set; }
    public int NumTotalLeves { get; set; }
    public int NeededAllowances { get; set; }

    // data from a certain Levequests Checklist spreadsheet
    // seem to be unused test levequests
    public static readonly uint[] ExcludedLeves = {
        502, 508, 514, 519, 525,
        531, 542, 544, 552, 554,
        562, 564, 582, 597, 822,
        827, 832, 871, 872, 877,
    };

    public FiltersState()
    {
        Reload();
    }

    public void Reload()
    {
        AllLeves = GetSheet<Leve>()
            .Where(row => row.LeveClient.Row != 0 && !ExcludedLeves.Contains(row.RowId))
            .ToArray();
    }
}
