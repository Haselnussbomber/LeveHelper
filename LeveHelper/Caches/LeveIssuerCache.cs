using System.Linq;
using HaselCommon.Cache;
using HaselCommon.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LeveHelper.Caches;

[RegisterSingleton]
public class LeveIssuerCache(ExcelService ExcelService) : MemoryCache<uint, RowRef<ENpcResident>[]>
{
    public override RowRef<ENpcResident>[]? CreateEntry(uint leveRowId)
        => LeveHelper.Data.Issuers
            .Where((kv) => kv.Value.Contains(leveRowId))
            .Distinct()
            .Select((kv) => ExcelService.CreateRef<ENpcResident>(kv.Key))
            .ToArray();
}
