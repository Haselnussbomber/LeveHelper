using System.Collections.Generic;
using System.Linq;
using AutoCtor;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Graphics;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using LeveHelper.Config;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.Columns;

[RegisterTransient, AutoConstruct]
public partial class TypeColumn : ColumnNumber<Leve>, IConnectedColumn<LeveListTable>
{
    private readonly PluginConfig _config;
    private readonly ExcelService _excelService;
    private readonly TextService _textService;
    private readonly TextureService _textureService;
    private readonly Dictionary<string, LeveAssignmentType[]> _groups = [];

    private LeveListTable _table = null!;
    private bool _popupOpen;

    [AutoPostConstruct]
    private void Initialize()
    {
        LabelKey = "ListTab.Column.Type";
        Flags = ImGuiTableColumnFlags.WidthFixed;
        Width = 200;

        LoadGroups();
    }

    public void SetTable(LeveListTable table)
    {
        _table = table;
    }

    private void LoadGroups()
    {
        _groups.Clear();

        // HowTo#69 => Fieldcraft Leves
        if (_excelService.TryGetRow<HowTo>(69, out var howTo))
            _groups.Add(howTo.Name.ExtractText(), CreateGroup(2, 3, 4));

        // HowTo#67 => Tradecraft Leves
        if (_excelService.TryGetRow(67, out howTo))
            _groups.Add(howTo.Name.ExtractText(), CreateGroup(5, 6, 7, 8, 9, 10, 11, 12));

        // HowTo#112 => Grand Company Leves
        if (_excelService.TryGetRow(112, out howTo))
            _groups.Add(howTo.Name.ExtractText(), CreateGroup(13, 14, 15));
    }

    private LeveAssignmentType[] CreateGroup(params uint[] rowIds)
    {
        var list = new List<LeveAssignmentType>();

        foreach (var rowId in rowIds)
        {
            if (_excelService.TryGetRow<LeveAssignmentType>(rowId, out var row))
                list.Add(row);
        }

        return [.. list.OrderBy(row => row.Name.ExtractText())];
    }

    public override int ToValue(Leve row)
    {
        return (int)row.LeveAssignmentType.RowId;
    }

    public override bool ShouldShow(Leve row)
    {
        if (_config.Filters.Type == 0)
            return true;

        return base.ShouldShow(row);
    }

    public override void OnLanguageChanged(string langCode)
    {
        LoadGroups();
    }

    public override void DrawColumn(Leve row)
    {
        var typeIcon = row.LeveAssignmentType.ValueNullable?.Icon ?? 0;
        if (typeIcon != 0)
        {
            ImGui.BeginGroup();
            _textureService.DrawIcon(typeIcon, ImGui.GetTextLineHeight());
            ImGui.SameLine();
            ImGui.TextUnformatted(row.LeveAssignmentType.ValueNullable?.Name.ExtractText() ?? string.Empty);
            ImGui.EndGroup();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip(_textService.Translate("ListTab.LeveType.Tooltip", row.LeveAssignmentType.ValueNullable?.Name ?? string.Empty));
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                SetValue(row.LeveAssignmentType.RowId);
            }
        }
    }

    public override unsafe bool DrawFilter()
    {
        using var id = ImRaii.PushId("##Filter");
        var all = _config.Filters.Type == 0;

        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        using var popupBorderColor = LeveListTable.ComboBorder.Push(ImGuiCol.Border);
        using var popupStyle = ImRaii.PushStyle(ImGuiStyleVar.PopupBorderSize, 1, _popupOpen)
            .Push(ImGuiStyleVar.FramePadding, itemSpacing)
            .Push(ImGuiStyleVar.WindowPadding, itemSpacing);
        _popupOpen = false;

        using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameRounding, 0);
        ImGui.SetNextItemWidth(-Table.ArrowWidth * ImGuiHelpers.GlobalScale);
        using var labelColor = Color.Green.Push(ImGuiCol.Text, !all);
        using var combo = ImRaii.Combo(string.Empty, Label, ImGuiComboFlags.NoArrowButton | ImGuiComboFlags.HeightLargest);

        labelColor.Pop();
        popupStyle.Pop();
        popupBorderColor.Pop();

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            SetValue(0);
            _table.ClosePopup = true;
            return true;
        }
        else if (ImGui.IsItemClicked(ImGuiMouseButton.Middle))
        {
            var suggestedType = GetSuggestedType();

            if (_config.Filters.Type != suggestedType)
            {
                SetValue((uint)suggestedType);
                return true;
            }

            return true;
        }

        if (ImGui.IsItemHovered())
        {
            var suggestedType = GetSuggestedType();
            var suggestedName = GetSuggestedTypeName(suggestedType);

            using var tooltip = ImRaii.Tooltip();

            if (_config.Filters.Type != 0)
                ImGui.TextUnformatted(_textService.Translate("ListTab.ColumnFilter.RightClickToResetFilters"));

            if (_config.Filters.Type != suggestedType)
                ImGui.TextUnformatted(_textService.Translate("ListTab.ColumnFilter.MiddleClickToSelect", suggestedName));
        }

        if (!combo)
            return false;

        _popupOpen = true;

        using var color = ImRaii.PushColor(ImGuiCol.Separator, (uint)LeveListTable.ComboBorder);
        ImGui.Spacing();

        // All
        if (ImGui.Selectable(_textService.Translate("ListTab.ColumnFilter.EnableAll") + "##All", all))
        {
            SetValue(0);
            return true;
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (all)
        {
            ImGui.SetItemDefaultFocus();
        }

        // Battlecraft
        if (_excelService.TryGetRow<LeveAssignmentType>(1, out var battleCraftType) && DrawComboOption(battleCraftType))
            return true;

        // Groups
        foreach (var group in _groups)
        {
            using (Color.Gold.Push(ImGuiCol.Text))
                ImGui.TextUnformatted(group.Key);

            using var indentStyle = ImRaii.PushStyle(ImGuiStyleVar.IndentSpacing, ImGui.GetStyle().ItemSpacing.X);

            ImGui.Separator();
            ImGui.Spacing();

            foreach (var type in group.Value)
            {
                if (DrawComboOption(type))
                    return true;
            }
        }

        return false;
    }

    private unsafe int GetSuggestedType()
    {
        return PlayerState.Instance()->CurrentClassJobId switch
        {
            16 => 2, // Miner
            17 => 3, // Botanist
            18 => 4, // Fisher

            8 => 5, // Carpenter
            9 => 6, // Blacksmith
            10 => 7, // Armorer
            11 => 8, // Goldsmith
            12 => 9, // Leatherworker
            13 => 10, // Weaver
            14 => 11, // Alchemist
            15 => 12, // Culinarian

            _ => 1, // Battlecraft -> any other job
        };
    }

    private unsafe string GetSuggestedTypeName(int suggestedType)
    {
        if (suggestedType == 1 && _excelService.TryGetRow<LeveAssignmentType>(1, out var suggestedBattleCraftType))
            return suggestedBattleCraftType.Name.ExtractText();

        if (_excelService.TryGetRow<ClassJob>(PlayerState.Instance()->CurrentClassJobId, out var classJob))
            return classJob.Name.ExtractText();

        return string.Empty;
    }

    private unsafe bool DrawComboOption(LeveAssignmentType type)
    {
        var clicked = ImGui.Selectable($"##Entry_{type.RowId}", _config.Filters.Type == type.RowId);

        if (type.Icon != 0)
        {
            ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X / 2f);
            _textureService.DrawIcon(type.Icon, 20);
        }

        ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X);
        ImGui.TextUnformatted(type.Name.ExtractText());

        if (clicked)
        {
            SetValue(type.RowId);
            return true;
        }

        if (_config.Filters.Type == type.RowId)
        {
            ImGui.SetItemDefaultFocus();
        }

        return false;
    }

    private void SetValue(uint rowId)
    {
        _config.Filters.Type = rowId;

        FilterNumber = (int)rowId;
        FilterRegex = null;

        _table.IsFilterDirty = true;
    }
}
