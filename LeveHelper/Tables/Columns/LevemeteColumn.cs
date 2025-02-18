using System.Linq;
using AutoCtor;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using HaselCommon.Graphics;
using HaselCommon.Gui;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using LeveHelper.Caches;
using LeveHelper.Config;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.Columns;

public record IssuerEntry(int Id, string Name, uint TerritoryType, string PlaceName);

[RegisterTransient, AutoConstruct]
public partial class LevemeteColumn : ColumnNumber<Leve>, IConnectedColumn<LeveListTable>
{
    private readonly PluginConfig _config;
    private readonly TextService _textService;
    private readonly MapService _mapService;
    private readonly IClientState _clientState;
    private readonly LeveIssuerCache _leveIssuerCache;
    private readonly ResidentLevelCache _residentLevelCache;

    private LeveListTable _table = null!;
    private bool _popupOpen;
    private IssuerEntry[] _issuers;

    [AutoPostConstruct]
    private void Initialize()
    {
        LabelKey = "ListTab.Column.Levemete";
        _issuers = GetIssuers();
    }

    public void SetTable(LeveListTable table)
    {
        _table = table;
    }

    public override string ToName(Leve row)
    {
        if (!_leveIssuerCache.TryGetValue(row.RowId, out var issuers))
            return string.Empty;

        return string.Join(", ", issuers.Select(issuer => _textService.GetENpcResidentName(issuer.RowId)));
    }

    public override void OnLanguageChanged(string langCode)
    {
        _issuers = GetIssuers();
    }

    public override bool ShouldShow(Leve row)
    {
        if (_config.Filters.Levemete == 0)
            return true;

        if (!_leveIssuerCache.TryGetValue(row.RowId, out var issuers))
            return false;

        return issuers.Any(issuer => issuer.RowId == _config.Filters.Levemete);
    }

    public override void DrawColumn(Leve row)
    {
        if (!_leveIssuerCache.TryGetValue(row.RowId, out var issuers))
            return;

        for (var i = 0; i < issuers.Length; i++)
        {
            var issuer = issuers[i];
            var level = _residentLevelCache.GetValue(issuer.RowId);

            ImGui.TextUnformatted(_textService.GetENpcResidentName(issuer.RowId));

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip(_textService.Translate(level.HasValue
                    ? "ListTab.Levemete.ContextMenu.Tooltip"
                    : "ListTab.Levemete.ContextMenu.TooltipNoMap"));
            }

            if (level.HasValue && ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                _mapService.OpenMap(level.Value);
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                SetFilter((int)issuer.RowId);
            }

            if (i < issuers.Length - 1)
            {
                ImGui.SameLine(0, 0);
                ImGui.TextUnformatted(",");
                ImGuiUtils.SameLineSpace();
            }
        }
    }

    public override unsafe bool DrawFilter()
    {
        using var id = ImRaii.PushId("##Filter");
        var any = _config.Filters.Levemete == 0;

        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        using var popupBorderColor = LeveListTable.ComboBorder.Push(ImGuiCol.Border);
        using var popupStyle = ImRaii.PushStyle(ImGuiStyleVar.PopupBorderSize, 1, _popupOpen)
            .Push(ImGuiStyleVar.FramePadding, itemSpacing)
            .Push(ImGuiStyleVar.WindowPadding, itemSpacing);
        _popupOpen = false;

        using var noFrameRounding = ImRaii.PushStyle(ImGuiStyleVar.FrameRounding, 0);
        ImGui.SetNextItemWidth(-Table.ArrowWidth * ImGuiHelpers.GlobalScale);
        using var labelColor = Color.Green.Push(ImGuiCol.Text, !any);
        using var combo = ImRaii.Combo(string.Empty, Label, ImGuiComboFlags.NoArrowButton | ImGuiComboFlags.HeightLarge);

        labelColor.Pop();
        popupStyle.Pop();
        popupBorderColor.Pop();

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            SetFilter(0);
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

        using var color = ImRaii.PushColor(ImGuiCol.Separator, (uint)LeveListTable.ComboBorder);
        ImGui.Spacing();

        var ret = false;

        // All
        if (ImGui.Selectable(_textService.Translate("ListTab.ColumnFilter.EnableAny") + "##Any", any))
        {
            SetFilter(0);
            ret = true;
        }

        ImGui.Separator();

        using var table = ImRaii.Table("LevemeteTable", 2, ImGuiTableFlags.RowBg);
        if (!table) return false;

        foreach (var issuer in _issuers)
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            var selected = issuer.Id == _config.Filters.Levemete;
            var clicked = ImGui.Selectable(issuer.Name, selected, ImGuiSelectableFlags.SpanAllColumns);
            if (selected)
            {
                ImGui.SetItemDefaultFocus();
            }

            ImGui.TableNextColumn();
            using (ImRaii.PushColor(ImGuiCol.Text, _clientState.TerritoryType == issuer.TerritoryType ? Color.White : ImGui.GetColorU32(ImGuiCol.TextDisabled)))
            {
                ImGui.TextUnformatted(issuer.PlaceName);
            }

            if (clicked)
            {
                SetFilter(issuer.Id);
                ret = true;
            }
        }

        return ret;
    }

    private IssuerEntry[] GetIssuers()
    {
        return LeveHelper.Data.Issuers.Keys
            .Select(residentId =>
            {
                var name = _textService.GetENpcResidentName(residentId);
                var level = _residentLevelCache.GetValue(residentId);
                var territoryTypeId = 0u;
                var placeName = string.Empty;

                if (level.HasValue && level.Value.Map.IsValid)
                {
                    territoryTypeId = level.Value.Map.Value.TerritoryType.RowId;
                    placeName = _textService.GetPlaceName(level.Value.Map.Value.PlaceName.RowId);
                }

                return new IssuerEntry((int)residentId, name, territoryTypeId, placeName);
            })
            .OrderBy(tuple => tuple.Name)
            .ToArray();
    }

    private void SetFilter(int residentId)
    {
        _config.Filters.Levemete = residentId;
        _config.Save();

        _table.IsFilterDirty = true;
    }
}
