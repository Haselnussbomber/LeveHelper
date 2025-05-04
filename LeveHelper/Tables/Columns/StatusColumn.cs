using System.Linq;
using AutoCtor;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using HaselCommon.Graphics;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using LeveHelper.Config;
using LeveHelper.Enums;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.Columns;

[RegisterTransient, AutoConstruct]
public partial class StatusColumn : Column<Leve>, IConnectedColumn<LeveListTable>
{
    private readonly PluginConfig _config;
    private readonly TextService _textService;
    private readonly LeveService _leveService;
    private readonly LeveStatus _anyFlags = Enum.GetValues<LeveStatus>().Aggregate((a, b) => a | b);

    private LeveListTable _table = null!;
    private bool _popupOpen;
    private LeveStatus[] _values;
    private string[] _names = [];
    private LeveStatus _filterValue;

    [AutoPostConstruct]
    private void Initialize()
    {
        LabelKey = "ListTab.Column.Status";
        Flags = ImGuiTableColumnFlags.WidthFixed;
        Width = 120;

        _filterValue = _anyFlags;
        _values = [.. Enum.GetValues<LeveStatus>()];
        UpdateNames();
    }

    public void SetTable(LeveListTable table)
    {
        _table = table;
    }

    public override void OnLanguageChanged(string langCode)
    {
        UpdateNames();
    }

    private void UpdateNames()
    {
        _names = Enum.GetValues<LeveStatus>().Select(status => _textService.Translate("StatusFilter.Status." + Enum.GetName(status))).ToArray();
    }

    public virtual unsafe LeveStatus ToStatus(Leve row)
    {
        if (_leveService.IsAccepted(row.RowId))
        {
            if (_leveService.IsFailed(row.RowId))
                return LeveStatus.Complete;
            else if (_leveService.IsReadyForTurnIn(row.RowId))
                return LeveStatus.Complete;
            else if (_leveService.IsStarted(row.RowId))
                return LeveStatus.Complete;

            return LeveStatus.Accepted;
        }
        else if (_leveService.IsComplete(row.RowId))
        {
            return LeveStatus.Complete;
        }

        return LeveStatus.Incomplete;
    }

    public override bool ShouldShow(Leve row)
    {
        var value = ToStatus(row);

        foreach (var status in Enum.GetValues<LeveStatus>())
        {
            if (_filterValue.HasFlag(status) && value == status)
                return true;
        }

        return false;
    }

    public override unsafe void DrawColumn(Leve row)
    {
        var value = ToStatus(row);
        using (ImRaii.PushColor(ImGuiCol.Text, value == LeveStatus.Complete ? Color.Green : (value == LeveStatus.Accepted ? Color.Yellow : Color.Red)))
            ImGui.TextUnformatted(_textService.Translate("StatusFilter.Status." + Enum.GetName(value)));
    }

    public override unsafe int Compare(Leve a, Leve b)
        => ToStatus(a).CompareTo(ToStatus(b));

    public override unsafe bool DrawFilter()
    {
        using var id = ImRaii.PushId("##Filter");
        var any = _filterValue.HasFlag(_anyFlags);

        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        using var popupBorderColor = LeveListTable.ComboBorder.Push(ImGuiCol.Border);
        using var popupStyle = ImRaii.PushStyle(ImGuiStyleVar.PopupBorderSize, 1, _popupOpen)
            .Push(ImGuiStyleVar.FramePadding, itemSpacing)
            .Push(ImGuiStyleVar.WindowPadding, itemSpacing);
        _popupOpen = false;

        using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameRounding, 0);
        ImGui.SetNextItemWidth(-HaselCommon.Gui.ImGuiTable.Table.ArrowWidth * ImGuiHelpers.GlobalScale);
        using var labelColor = Color.Green.Push(ImGuiCol.Text, !any);
        using var combo = ImRaii.Combo(string.Empty, Label, ImGuiComboFlags.NoArrowButton | ImGuiComboFlags.HeightLargest);

        labelColor.Pop();
        popupStyle.Pop();
        popupBorderColor.Pop();

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            SetValue(_anyFlags, true);
            _table.ClosePopup = true;
            return true;
        }

        if (!any && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(_textService.Translate("ListTab.ColumnFilter.RightClickToResetFilters"));
        }

        if (!combo)
            return false;

        _popupOpen = true;
        var ret = false;

        using var separatorColor = ImRaii.PushColor(ImGuiCol.Separator, LeveListTable.ComboBorder);
        ImGui.Spacing();

        // All
        if (ImGui.Checkbox(_textService.Translate("ListTab.ColumnFilter.EnableAny") + "##Any", ref any))
        {
            SetValue(_anyFlags, any);
            ret = true;
        }

        ImGui.Separator();

        using var indent = ImRaii.PushIndent(10f);
        for (var i = 1; i < _names.Length; ++i)
        {
            var tmp = _filterValue.HasFlag(_values[i]);

            if (!ImGui.Checkbox(_names[i], ref tmp))
                continue;

            if (tmp)
                ImGui.SetItemDefaultFocus();

            SetValue(_values[i], tmp);
            ret = true;
        }

        return ret;
    }

    private void SetValue(LeveStatus value, bool enable)
    {
        if (enable)
            _filterValue |= value;
        else
            _filterValue &= ~value;

        _config.Filters.Status = _filterValue;
        _config.Save();

        _table.IsFilterDirty = true;
    }
}
