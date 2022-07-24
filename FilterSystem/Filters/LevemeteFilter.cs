using System.Linq;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Filters;

public class LevemeteFilter : Filter
{
    public LevemeteFilter(FilterManager manager) : base(manager)
    {
    }

    public Configuration Config => Service.Config.Filters.LevemeteFilter;

    public class Configuration
    {
        public uint SelectedLevemete = 0;
    }

    public override void Reset()
    {
        Config.SelectedLevemete = 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedLevemete = (uint)value;
        Service.Config.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Levemete:");

        ImGui.TableNextColumn();
        if (ImGui.BeginCombo("##LeveHelper_LevemeteFilter_Combo", state.levemetes.ContainsKey(Config.SelectedLevemete) ? state.levemetes[Config.SelectedLevemete] : "All"))
        {
            if (ImGui.Selectable("All##LeveHelper_LevemeteFilter_Combo", Config.SelectedLevemete == 0))
            {
                Set(0);
                manager.Update();
            }

            if (Config.SelectedLevemete == 0)
                ImGui.SetItemDefaultFocus();

            foreach (var kv in state.levemetes)
            {
                if (ImGui.Selectable(kv.Value + "##LeveHelper_LevemeteFilter_Combo", Config.SelectedLevemete == kv.Key))
                {
                    Set(kv.Key);
                    manager.Update();
                }

                if (Config.SelectedLevemete == kv.Key)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
    }

    public override bool Run()
    {
        var ENpcResidentSheet = Service.Data.GetExcelSheet<ENpcResident>();
        state.levemetes = state.leves
            .Select(row => row.leve.LevelLevemete.Value?.Object)
            .Where(item => item != null)
            .Cast<uint>()
            .GroupBy(item => item)
            .Select(group => ENpcResidentSheet?.GetRow(group.First()))
            .Where(item => item != null)
            .Cast<ENpcResident>()
            .Select(item => (item.RowId, Name: item.Singular.ClearString()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLevemete == 0)
            return false;

        state.leves = state.leves.Where(item => item.leve.LevelLevemete.Value?.Object == Config.SelectedLevemete);

        return true;
    }
}
