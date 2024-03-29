using System.Linq;
using Dalamud.Interface.Utility.Raii;
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

    public static NameFilterConfiguration Config => Service.GetService<Configuration>().Filters.NameFilter;

    public override void Reload()
    {
    }

    public override void Reset()
    {
        Config.CurrentName = "";
    }

    public override bool HasValue()
    {
        return Config.CurrentName != "";
    }

    public override void Set(dynamic value)
    {
        Config.CurrentName = (string)value;
        Service.GetService<Configuration>().Save();
    }

    public override void Draw()
    {
        using var id = ImRaii.PushId("NameFilter");

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(t("NameFilter.Label"));

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(InputWidth);
        var currentName = Config.CurrentName;
        if (ImGui.InputText("##Input", ref currentName, 255))
        {
            Set(currentName);
            manager.Update();
        }
    }

    public override bool Run()
    {
        if (string.IsNullOrWhiteSpace(Config.CurrentName))
            return false;

        state.Leves = state.Leves.Where(row => row.Name.Contains(Config.CurrentName, StringComparison.InvariantCultureIgnoreCase));

        return true;
    }
}
