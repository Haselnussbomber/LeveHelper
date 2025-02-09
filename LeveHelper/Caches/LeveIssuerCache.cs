using System.Collections.Generic;
using System.Linq;
using HaselCommon.Cache;
using HaselCommon.Services;
using Lumina.Excel.Sheets;

namespace LeveHelper.Caches;

[RegisterSingleton]
public class LeveIssuerCache(ExcelService ExcelService) : MemoryCache<uint, ENpcResident[]>
{
    public override ENpcResident[]? CreateEntry(uint leveRowId)
    {
        var residents = new Dictionary<uint, ENpcResident>();

        foreach (var entry in LeveHelper.Data.Issuers
            .Where((kv) => kv.Value.Contains(leveRowId))
            .Distinct())
        {
            if (!residents.ContainsKey(entry.Key) && ExcelService.TryGetRow<ENpcResident>(entry.Key, out var resident))
            {
                residents.Add(entry.Key, resident);
            }
        }

        return [.. residents.Values];
    }
}
