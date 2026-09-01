using System.Linq;
using HaselCommon.Services;
using LeveHelper.Utils;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LeveHelper.Caches;

[RegisterSingleton]
public class AssignmentIssuerCache(ExcelService ExcelService) : MemoryCache<uint, RowRef<ENpcBase>>
{
    public override RowRef<ENpcBase> CreateEntry(uint guildleveAssignmentId)
    {
        if (!ExcelService.TryFindRow<ENpcBase>(row => row.ENpcData.Any(rowRef => rowRef.RowId == guildleveAssignmentId), out var eNpcBase))
            return default;

        return ExcelService.CreateRowRef<ENpcBase>(eNpcBase.RowId);
    }
}
