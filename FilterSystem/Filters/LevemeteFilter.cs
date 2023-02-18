using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Filters;

public class LevemeteFilterConfiguration
{
    public uint SelectedLevemete = 0;
}

public class LevemeteFilter : Filter
{
    public LevemeteFilter(FilterManager manager) : base(manager)
    {
    }

    public static LevemeteFilterConfiguration Config => Plugin.Config.Filters.LevemeteFilter;

    private Dictionary<uint, string>? Levemetes = null;

    public override void Reset()
    {
        Config.SelectedLevemete = 0;
    }

    public override bool HasValue()
    {
        return Config.SelectedLevemete != 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedLevemete = (uint)value;
        Plugin.Config.Save();
    }

    public override void Draw()
    {
        if (Levemetes == null)
            return;

        ImGui.TableNextColumn();
        ImGui.Text("Levemete:");

        ImGui.TableNextColumn();
        if (ImGui.BeginCombo("##LeveHelper_LevemeteFilter_Combo", Levemetes.TryGetValue(Config.SelectedLevemete, out var value) ? value : "All"))
        {
            if (ImGui.Selectable("All##LeveHelper_LevemeteFilter_Combo_0", Config.SelectedLevemete == 0))
            {
                Set(0);
                manager.Update();
            }

            if (Config.SelectedLevemete == 0)
            {
                ImGui.SetItemDefaultFocus();
            }

            foreach (var kv in Levemetes)
            {
                if (ImGui.Selectable($"{kv.Value}##LeveHelper_LevemeteFilter_Combo_{kv.Key}", Config.SelectedLevemete == kv.Key))
                {
                    Set(kv.Key);
                    manager.Update();
                }

                if (Config.SelectedLevemete == kv.Key)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }
    }

    public override bool Run()
    {
        var ENpcResidentSheet = Service.Data.GetExcelSheet<ENpcResident>();
        Levemetes = state.Leves
            .Select(row => row.Leve?.LevelLevemete.Value?.Object)
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

        var selection = state.Leves.Where(item => item.Leve?.LevelLevemete.Value?.Object == Config.SelectedLevemete);
        if (!selection.Any())
        {
            Config.SelectedLevemete = 0;
            return false;
        }

        state.Leves = selection;

        return true;
    }
}
