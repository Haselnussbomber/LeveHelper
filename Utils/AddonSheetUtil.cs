using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Utils;

internal class AddonSheetUtil
{
    private static Dictionary<uint, string> cache = new();

    public static string GetText(uint rowId, string? fallback = null)
    {
        return cache.ContainsKey(rowId)
            ? cache[rowId]
            : cache[rowId] = Service.Data.GetExcelSheet<Addon>()?.GetRow(rowId)?.Text ?? fallback ?? "";
    }
}
