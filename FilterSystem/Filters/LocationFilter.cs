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

    public static LocationFilterConfiguration Config => Configuration.Instance.Filters.LocationFilter;

    public override void Reset()
    {
        Config.SelectedLocation = 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedLocation = (uint)value;
        Configuration.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Location:");

        ImGui.TableNextColumn();
        if (ImGui.BeginCombo("##LeveHelper_LocationFilter_Combo", state.locations.ContainsKey(Config.SelectedLocation) ? state.locations[Config.SelectedLocation] : "All"))
        {
            if (ImGui.Selectable("All##LeveHelper_LocationFilter_Combo", Config.SelectedLocation == 0))
            {
                Set(0);
                manager.Update();
            }

            if (Config.SelectedLocation == 0)
                ImGui.SetItemDefaultFocus();

            foreach (var kv in state.locations)
            {
                if (ImGui.Selectable(kv.Value + "##LeveHelper_LocationFilter_Combo", Config.SelectedLocation == kv.Key))
                {
                    Set(kv.Key);
                    manager.Update();
                }

                if (Config.SelectedLocation == kv.Key)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        ImGui.SameLine();

        var placeNameId = Service.PlaceNameService.PlaceNameId;
        if (placeNameId != 0 && state.AllLocations.Contains(placeNameId) && ImGui.Button("Set Current Zone"))
        {
            Config.SelectedLocation = placeNameId;
            manager.Update();
        }
    }

    public override bool Run()
    {
        state.locations = state.leves
            .Select(row => row.leve.PlaceNameStartZone.Value)
            .Where(item => item != null)
            .Cast<PlaceName>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First())
            .Select(item => (item.RowId, Name: item.Name.ClearString()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLocation == 0)
            return false;

        state.leves = state.leves.Where(item => item.leve.PlaceNameStartZone.Row == Config.SelectedLocation);

        return true;
    }
}
