using HaselCommon.Gui.ImGuiTable;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.LeveListTableColumns;

[RegisterTransient]
public class RowIdColumn : ColumnNumber<Leve>
{
    public RowIdColumn()
    {
        LabelKey = "ListTab.Column.Id";
        Flags = ImGuiTableColumnFlags.WidthFixed;
        Width = 50;
    }

    public override int ToValue(Leve row)
        => (int)row.RowId;
}
