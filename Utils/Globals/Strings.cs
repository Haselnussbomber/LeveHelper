using System.Linq;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel;
using AddonSheet = Lumina.Excel.GeneratedSheets.Addon;

namespace LeveHelper.Utils.Globals;

public static unsafe class Strings
{
    public static string t(string key)
        => Service.TranslationManager.Translate(key);

    public static string t(string key, params object?[] args)
        => Service.TranslationManager.Translate(key, args);

    public static SeString tSe(string key, params SeString[] args)
        => Service.TranslationManager.TranslateSeString(key, args.Select(s => s.Payloads).ToArray());

    public static string GetAddonText(uint id)
        => Service.StringManager.GetSheetText<AddonSheet>(id, "Text");

    public static string GetSheetText<T>(uint rowId, string columnName) where T : ExcelRow
        => Service.StringManager.GetSheetText<T>(rowId, columnName);
}
