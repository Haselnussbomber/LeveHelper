using System.Linq;
using HaselCommon.Caching;
using HaselCommon.Services;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Caches;

public class LeveIssuerCache(ExcelService ExcelService) : MemoryCache<uint, ENpcResident[]>
{
    public override ENpcResident[]? CreateEntry(uint leveRowId)
        => LeveHelper.Data.Issuers
            .Where((kv) => kv.Value.Contains(leveRowId))
            .Distinct()
            .Select((kv) => ExcelService.GetRow<ENpcResident>(kv.Key)!)
            .ToArray();
}
