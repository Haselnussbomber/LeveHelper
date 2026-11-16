namespace LeveHelper.Caches;

[RegisterSingleton]
public class ResidentLevelCache(ExcelService ExcelService) : MemoryCache<uint, Level?>
{
    public override Level? CreateEntry(uint residentId)
    {
        return ExcelService.TryFindRow<Level>(row => row.Type == 8 && row.Object.RowId == residentId, out var levelRow)
            ? levelRow
            : null;
    }
}
