using System.Collections.Generic;
using System.Linq;
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

    public static LocationFilterConfiguration Config => Plugin.Config.Filters.LocationFilter;

    private Dictionary<uint, string>? Locations = null;

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
        if (Locations == null)
            return;

        ImGui.TableNextColumn();
        ImGui.Text("Location:");

        ImGui.TableNextColumn();
        if (ImGui.BeginCombo("##LeveHelper_LocationFilter_Combo", Locations.TryGetValue(Config.SelectedLocation, out var value) ? value : "All"))
        {
            if (ImGui.Selectable("All##LeveHelper_LocationFilter_Combo_0", Config.SelectedLocation == 0))
            {
                Set(0);
                manager.Update();
            }

            if (Config.SelectedLocation == 0)
            {
                ImGui.SetItemDefaultFocus();
            }

            foreach (var kv in Locations)
            {
                if (ImGui.Selectable($"{kv.Value}##LeveHelper_LocationFilter_Combo_{kv.Key}", Config.SelectedLocation == kv.Key))
                {
                    Set(kv.Key);
                    manager.Update();
                }

                if (Config.SelectedLocation == kv.Key)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        ImGui.SameLine();

        var placeNameId = PlaceNameHelper.PlaceNameId;
        if (placeNameId != 0 && Config.SelectedLocation != placeNameId && Locations.ContainsKey(placeNameId) && ImGui.Button("Set Current Zone"))
        {
            Config.SelectedLocation = placeNameId;
            manager.Update();
        }
    }

    public override bool Run()
    {
        Locations = state.Leves
            .Select(row => row.Leve?.PlaceNameStartZone.Value)
            .Where(item => item != null)
            .Cast<PlaceName>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First())
            .Select(item => (item.RowId, Name: item.Name.ClearString()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLocation == 0)
            return false;

        var selection = state.Leves.Where(item => item.Leve?.PlaceNameStartZone.Row == Config.SelectedLocation);
        if (!selection.Any())
        {
            Config.SelectedLocation = 0;
            return false;
        }

        state.Leves = selection;

        return true;
    }
}
