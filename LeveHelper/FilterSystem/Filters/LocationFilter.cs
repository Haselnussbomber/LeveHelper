using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Config;
using LeveHelper.Interfaces;
using Lumina.Excel.Sheets;

namespace LeveHelper.Filters;

public class LocationFilterConfiguration
{
    public uint SelectedLocation = 0;
}

public class LocationFilter(PluginConfig PluginConfig, TextService TextService, ExcelService ExcelService) : IFilter
{
    public int Order => 3;
    private LocationFilterConfiguration Config => PluginConfig.Filters.LocationFilter;

    public FilterManager? FilterManager { get; set; }

    private Dictionary<uint, string>? Locations { get; set; }
    private uint LastTerritoryId { get; set; }
    private uint CurrentPlaceNameId { get; set; }

    public void Reload()
    {
        Locations?.Clear();
        Run();
    }

    public void Reset()
    {
        Config.SelectedLocation = 0;
    }

    public bool HasValue()
    {
        return Config.SelectedLocation != 0;
    }

    public void Set(dynamic value)
    {
        Config.SelectedLocation = (uint)value;
        PluginConfig.Save();
    }

    public void Draw()
    {
        if (Locations == null)
            return;

        using var id = ImRaii.PushId("LocationFilter");

        ImGui.TableNextColumn();
        TextService.Draw("LocationFilter.Label");

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(250);
        using (var combo = ImRaii.Combo("##Combo", Locations.TryGetValue(Config.SelectedLocation, out var value) ? value : TextService.Translate("LocationFilter.Selectable.All")))
        {
            if (combo.Success)
            {
                if (ImGui.Selectable(TextService.Translate("LocationFilter.Selectable.All") + "##All", Config.SelectedLocation == 0))
                {
                    Set(0);
                    FilterManager!.Update();
                }

                if (Config.SelectedLocation == 0)
                {
                    ImGui.SetItemDefaultFocus();
                }

                foreach (var kv in Locations)
                {
                    if (ImGui.Selectable($"{kv.Value}##Entry{kv.Key}", Config.SelectedLocation == kv.Key))
                    {
                        Set(kv.Key);
                        FilterManager!.Update();
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
            if (LastTerritoryId != territoryId)
            {
                LastTerritoryId = territoryId;
                CurrentPlaceNameId = ExcelService.TryGetRow<TerritoryType>(territoryId, out var territoryType) ? territoryType.PlaceName.RowId : 0;
            }
        }

        if (CurrentPlaceNameId != 0 &&
            Config.SelectedLocation != CurrentPlaceNameId &&
            Locations.ContainsKey(CurrentPlaceNameId) &&
            ImGui.Button(TextService.Translate("LocationFilter.SetCurrentZone")))
        {
            Set(CurrentPlaceNameId);
            FilterManager!.Update();
        }
    }

    public bool Run()
    {
        Locations = FilterManager!.State.Leves
            .Select(row => row.PlaceNameStartZone)
            .Where(rowRef => rowRef.IsValid)
            .Select(rowRef => rowRef.Value)
            .Cast<PlaceName>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First())
            .Select(item => (item.RowId, Name: item.Name.ExtractText()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLocation == 0)
            return false;

        var selection = FilterManager.State.Leves.Where(item => item.PlaceNameStartZone.RowId == Config.SelectedLocation);
        if (!selection.Any())
        {
            Config.SelectedLocation = 0;
            return false;
        }

        FilterManager.State.Leves = selection;

        return true;
    }
}
