using AutoCtor;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Graphics;
using HaselCommon.Gui;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using LeveHelper.Config;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.Columns;

[RegisterTransient, AutoConstruct]
public partial class LevelColumn : ColumnNumber<Leve>, IConnectedColumn<LeveListTable>
{
    private readonly PluginConfig _config;
    private readonly TextService _textService;

    private LeveListTable _table = null!;
    private bool _popupOpen;

    [AutoPostConstruct]
    private void Initialize()
    {
        LabelKey = "ListTab.Column.Level";
        Flags = ImGuiTableColumnFlags.WidthFixed;
        Width = 50;
    }

    public void SetTable(LeveListTable table)
    {
        _table = table;
    }

    public override int ToValue(Leve row)
        => row.ClassJobLevel;

    public override bool ShouldShow(Leve row)
    {
        if (_config.Filters.MinLevel == 0 && _config.Filters.MaxLevel == 0)
            return true;

        var show = true;

        if (_config.Filters.MinLevel != 0)
            show &= row.ClassJobLevel >= _config.Filters.MinLevel;

        if (_config.Filters.MaxLevel != 0)
            show &= row.ClassJobLevel <= _config.Filters.MaxLevel;

        return show;
    }

    public override unsafe bool DrawFilter()
    {
        using var id = ImRaii.PushId("##Filter");
        var any = _config.Filters.MinLevel == 0 && _config.Filters.MaxLevel == 0;

        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        using var popupBorderColor = LeveListTable.ComboBorder.Push(ImGuiCol.Border);
        using var popupStyle = ImRaii.PushStyle(ImGuiStyleVar.PopupBorderSize, 1, _popupOpen)
            .Push(ImGuiStyleVar.FramePadding, itemSpacing)
            .Push(ImGuiStyleVar.WindowPadding, itemSpacing);
        _popupOpen = false;

        using var noFrameRounding = ImRaii.PushStyle(ImGuiStyleVar.FrameRounding, 0);
        ImGui.SetNextItemWidth(-HaselCommon.Gui.ImGuiTable.Table.ArrowWidth * ImGuiHelpers.GlobalScale);
        using var labelColor = Color.Green.Push(ImGuiCol.Text, !any);
        using var combo = ImRaii.Combo(string.Empty, Label, ImGuiComboFlags.NoArrowButton | ImGuiComboFlags.HeightLarge);

        labelColor.Pop();
        popupStyle.Pop();
        popupBorderColor.Pop();

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            _config.Filters.MinLevel = 0;
            _config.Filters.MaxLevel = 0;
            _config.Save();
            _table.ClosePopup = true;
            _table.IsFilterDirty = true;
            return true;
        }

        if (!any && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(_textService.Translate("ListTab.ColumnFilter.RightClickToResetFilters"));
        }

        if (!combo)
            return false;

        _popupOpen = true;

        var playerState = PlayerState.Instance();
        var maxLevel = playerState->IsLoaded == 1 ? playerState->MaxLevel : 100;

        void DrawInput(ref bool changed, string key, ref int value)
        {
            ImGui.TextUnformatted(_textService.Translate($"LevelFilter.{key}.Label"));

            ImGui.SetNextItemWidth(250);
            changed |= ImGui.SliderInt("###Input" + key, ref value, 0, maxLevel);

            ImGui.SameLine();
            if (ImGuiUtils.IconButton("Reset###Reset" + key, FontAwesomeIcon.Undo, _textService.GetAddonText(4830) ?? "Reset"))
            {
                value = 0;
                changed |= true;
            }
        }

        var changed = false;

        ImGui.Spacing();

        DrawInput(ref changed, "MinLevel", ref _config.Filters.MinLevel);

        ImGui.Spacing();

        DrawInput(ref changed, "MaxLevel", ref _config.Filters.MaxLevel);

        ImGui.Spacing();

        if (changed)
        {
            _config.Save();
            _table.IsFilterDirty = true;
        }

        return changed;
    }
}
