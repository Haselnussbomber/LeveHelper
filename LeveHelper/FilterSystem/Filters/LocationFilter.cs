using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Filters;

public class LocationFilterConfiguration
{
    public uint SelectedLocation = 0;
}

public class LocationFilter : Filter
{
    public LocationFilter(FilterManager manager) : base(manager)
    {
    }

    private static LocationFilterConfiguration Config => Plugin.Config.Filters.LocationFilter;

    private Dictionary<uint, string>? _locations { get; set; }
    private uint _lastTerritoryId { get; set; }
    private uint _currentPlaceNameId { get; set; }

    public override void Reload()
    {
        _locations?.Clear();
        Run();
    }

    public override void Reset()
    {
        Config.SelectedLocation = 0;
    }

    public override bool HasValue()
    {
        return Config.SelectedLocation != 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedLocation = (uint)value;
        Plugin.Config.Save();
    }

    public override void Draw()
    {
        if (_locations == null)
            return;

        using var id = ImRaii.PushId("LocationFilter");

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(t("LocationFilter.Label"));

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(InputWidth);
        using (var combo = ImRaii.Combo("##Combo", _locations.TryGetValue(Config.SelectedLocation, out var value) ? value : t("LocationFilter.Selectable.All")))
        {
            if (combo.Success)
            {
                if (ImGui.Selectable(t("LocationFilter.Selectable.All") + "##All", Config.SelectedLocation == 0))
                {
                    Set(0);
                    manager.Update();
                }

                if (Config.SelectedLocation == 0)
                {
                    ImGui.SetItemDefaultFocus();
                }

                foreach (var kv in _locations)
                {
                    if (ImGui.Selectable($"{kv.Value}##Entry{kv.Key}", Config.SelectedLocation == kv.Key))
                    {
                        Set(kv.Key);
                        manager.Update();
                    }

                    if (Config.SelectedLocation == kv.Key)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
            }
        }

        ImGui.SameLine();

        unsafe
        {
            var territoryId = GameMain.Instance()->CurrentTerritoryTypeId;
            if (_lastTerritoryId != territoryId)
            {
                _lastTerritoryId = territoryId;
                _currentPlaceNameId = GetRow<TerritoryType>(territoryId)?.PlaceName?.Row ?? 0;
            }
        }

        if (_currentPlaceNameId != 0 &&
            Config.SelectedLocation != _currentPlaceNameId &&
            _locations.ContainsKey(_currentPlaceNameId) &&
            ImGui.Button(t("LocationFilter.SetCurrentZone")))
        {
            Config.SelectedLocation = _currentPlaceNameId;
            manager.Update();
        }
    }

    public override bool Run()
    {
        _locations = state.Leves
            .Select(row => row.PlaceNameStartZone.Value)
            .Where(item => item != null)
            .Cast<PlaceName>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First())
            .Select(item => (item.RowId, Name: item.Name.ToDalamudString().ToString()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLocation == 0)
            return false;

        var selection = state.Leves.Where(item => item.PlaceNameStartZone.Row == Config.SelectedLocation);
        if (!selection.Any())
        {
            Config.SelectedLocation = 0;
            return false;
        }

        state.Leves = selection;

        return true;
    }
}
