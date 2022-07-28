using System;
using System.Linq;
using ImGuiNET;

namespace LeveHelper.Filters;

public class NameFilterConfiguration
{
    public string CurrentName = "";
}

public class NameFilter : Filter
{
    public NameFilter(FilterManager manager) : base(manager)
    {
    }

    public static NameFilterConfiguration Config => Configuration.Instance.Filters.NameFilter;

    public override void Reset()
    {
        Config.CurrentName = "";
    }

    public override void Set(dynamic value)
    {
        Config.CurrentName = (string)value;
        Configuration.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Name:");

        ImGui.TableNextColumn();
        var currentName = Config.CurrentName;
        if (ImGui.InputText("##LeveHelper_NameFilter_Input", ref currentName, 255))
        {
            Set(currentName);
            manager.Update();
        }
    }

    public override bool Run()
    {
        if (string.IsNullOrWhiteSpace(Config.CurrentName))
            return false;

        state.leves = state.leves.Where(row => row.Name.Contains(Config.CurrentName, StringComparison.InvariantCultureIgnoreCase));

        return true;
    }
}
