using System.Linq;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Filters;

public class ClassFilter : Filter
{
    public ClassFilter(FilterManager manager) : base(manager)
    {
    }

    public Configuration Config => Service.Config.Filters.ClassFilter;

    public class Configuration
    {
        public uint SelectedClass = 0;
    }

    public override void Reset()
    {
        Config.SelectedClass = 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedClass = (uint)value;
        Service.Config.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Class:");

        ImGui.TableNextColumn();
        if (ImGui.BeginCombo("##LeveHelper_ClassFilter_Combo", state.classes.ContainsKey(Config.SelectedClass) ? state.classes[Config.SelectedClass] : "All"))
        {
            if (ImGui.Selectable("All##LeveHelper_ClassFilter_Combo", Config.SelectedClass == 0))
            {
                Set(0);
                manager.Update();
            }

            if (Config.SelectedClass == 0)
                ImGui.SetItemDefaultFocus();

            foreach (var kv in state.classes)
            {
                if (ImGui.Selectable(kv.Value + "##LeveHelper_ClassFilter_Combo", Config.SelectedClass == kv.Key))
                {
                    Set(kv.Key);
                    manager.Update();
                }

                if (Config.SelectedClass == kv.Key)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
    }

    public override bool Run()
    {
        var LeveAssignmentTypeSheet = Service.Data.GetExcelSheet<LeveAssignmentType>();
        state.classes = state.leves
            .Select(item => item.value.Unknown4 != 0 ? LeveAssignmentTypeSheet?.GetRow((uint)item.value.Unknown4) : null)
            .Where(item => item != null)
            .Cast<LeveAssignmentType>()
            .GroupBy(item => item.RowId)
            .Select(group => group.First())
            .Where(item => item != null)
            .Select(item => (item.RowId, Name: item.Name.ClearString()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedClass == 0)
            return false;

        state.leves = state.leves.Where(item => item.value.Unknown4 == Config.SelectedClass);

        return true;
    }
}
