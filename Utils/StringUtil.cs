using System.Collections.Generic;
using System.Globalization;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

internal class StringUtil
{
    private static readonly Dictionary<(string, uint), string> cache = new();

    public static string GetText(string sheet, uint rowId, string? fallback = null)
    {
        var key = (sheet, rowId);

        if (!cache.TryGetValue(key, out var value))
        {
            var text = sheet switch
            {
                "Addon" => Service.Data.GetExcelSheet<Addon>()?.GetRow(rowId)?.Text.ClearString(),
                "Completion" => Service.Data.GetExcelSheet<Completion>()?.GetRow(rowId)?.Text.ClearString(),
                "HowTo" => Service.Data.GetExcelSheet<HowTo>()?.GetRow(rowId)?.Name.ClearString(),
                "LeveAssignmentType" => Service.Data.GetExcelSheet<LeveAssignmentType>()?.GetRow(rowId)?.Name.ClearString(),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(text))
            {
                text = fallback ?? "";
            }

            value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
            cache.Add(key, value);
        }

        return value;
    }
}
