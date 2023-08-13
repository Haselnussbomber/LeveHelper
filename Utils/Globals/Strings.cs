using Lumina.Excel;

namespace LeveHelper.Utils.Globals;

public static unsafe class Strings
{
    public static string GetAddonText(uint id)
        => Service.StringManager.GetAddonText(id);

    public static string GetSheetText<T>(uint rowId, string columnName) where T : ExcelRow
        => Service.StringManager.GetSheetText<T>(rowId, columnName);
}
