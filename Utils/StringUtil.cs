using System.Collections.Generic;
using System.Globalization;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

internal class StringUtil
{
    private static readonly Dictionary<uint, string> AddonCache = new();
    private static readonly Dictionary<(string, uint), string> SheetTextCache = new();

    public static string GetAddonText(uint rowId)
    {
        if (!AddonCache.TryGetValue(rowId, out var value))
        {
            unsafe
            {
                var ptr = (nint)Framework.Instance()->GetUiModule()->GetRaptureTextModule()->GetAddonText(rowId);
                if (ptr != 0)
                {
                    value = MemoryHelper.ReadSeStringNullTerminated(ptr).ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(value))
                return "";

            AddonCache.Add(rowId, value);
        }

        return value;
    }

    public static string GetText(string sheet, uint rowId, string? fallback = null)
    {
        var key = (sheet, rowId);

        if (!SheetTextCache.TryGetValue(key, out var value))
        {
            var text = sheet switch
            {
                "Addon" => Service.Data.GetExcelSheet<Addon>()?.GetRow(rowId)?.Text.ClearString(),
                "ClassJob" => Service.Data.GetExcelSheet<ClassJob>()?.GetRow(rowId)?.Name.ClearString(),
                "Completion" => Service.Data.GetExcelSheet<Completion>()?.GetRow(rowId)?.Text.ClearString(),
                "HowTo" => Service.Data.GetExcelSheet<HowTo>()?.GetRow(rowId)?.Name.ClearString(),
                "LeveAssignmentType" => Service.Data.GetExcelSheet<LeveAssignmentType>()?.GetRow(rowId)?.Name.ClearString(),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(text))
                text = fallback ?? "";

            value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
            SheetTextCache.Add(key, value);
        }

        return value;
    }
}
