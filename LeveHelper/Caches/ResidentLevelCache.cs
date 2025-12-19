using HaselCommon.Services;
using LeveHelper.Utils;
using Lumina.Excel.Sheets;

namespace LeveHelper.Caches;

[RegisterSingleton]
public class ResidentLevelCache(ExcelService ExcelService) : MemoryCache<uint, Level?>
{
    public override Level? CreateEntry(uint residentId)
    {
        return ExcelService.TryFindRow<Level>(row => row.Object.RowId == residentId, out var levelRow)
            ? levelRow
            : null;
    }
}
