using AutoCtor;
using HaselCommon.Gui.ImGuiTable;
using LeveHelper.Config;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.Columns;

[RegisterTransient, AutoConstruct]
public partial class RowIdColumn : ColumnNumber<Leve>
{
    private readonly PluginConfig _config;

    [AutoPostConstruct]
    private void Initialize()
    {
        LabelKey = "ListTab.Column.Id";
        Flags = ImGuiTableColumnFlags.WidthFixed;
        Width = 50;

        FilterNumber = _config.Filters.RowId == 0 ? null : (int)_config.Filters.RowId;
    }

    public override int ToValue(Leve row)
        => (int)row.RowId;

    public override bool DrawFilter()
    {
        if (!base.DrawFilter())
            return false;

        _config.Filters.RowId = (uint)(FilterNumber ?? 0);
        _config.Save();

        return true;
    }
}
