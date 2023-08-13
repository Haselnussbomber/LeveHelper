using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel;

namespace LeveHelper.Services;

public unsafe class StringManager : IDisposable
{
    private readonly Dictionary<uint, string> _addonCache = new();
    private readonly Dictionary<(string sheetName, uint rowId, string columnName), string> _sheetCache = new();

    public string GetAddonText(uint id)
    {
        if (!_addonCache.TryGetValue(id, out var value))
        {
            var ptr = (nint)RaptureTextModule.Instance()->GetAddonText(id);
            if (ptr != 0)
            {
                value = MemoryHelper.ReadSeStringNullTerminated(ptr).ToString();

                if (string.IsNullOrWhiteSpace(value))
                    return null ?? $"[Addon#{id}]";

                _addonCache.Add(id, value);
            }
        }

        return value ?? $"[Addon#{id}]";
    }

    public string GetSheetText<T>(uint rowId, string columnName) where T : ExcelRow
    {
        var sheetType = typeof(T);
        var sheetName = sheetType.Name;
        var key = (sheetName, rowId, columnName);

        if (!_sheetCache.TryGetValue(key, out var value))
        {

            var prop = sheetType.GetProperty(columnName);
            if (prop == null || prop.PropertyType != typeof(Lumina.Text.SeString))
                return string.Empty;

            var sheetRow = GetRow<T>(rowId);
            if (sheetRow == null)
                return string.Empty;

            var seStr = (Lumina.Text.SeString?)prop.GetValue(sheetRow);
            if (seStr == null)
                return string.Empty;

            value = SeString.Parse(seStr.RawData).ToString();

            _sheetCache.Add(key, value);
        }

        return value;
    }

    public void Dispose()
    {
        _addonCache.Clear();
        _sheetCache.Clear();
    }
}
