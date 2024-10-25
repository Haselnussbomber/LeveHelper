using System.Linq;
using Dalamud.Interface.Utility.Raii;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Config;
using LeveHelper.Interfaces;

namespace LeveHelper.Filters;

public class NameFilterConfiguration
{
    public string CurrentName = "";
}

public class NameFilter(PluginConfig PluginConfig, TextService TextService) : IFilter
{
    public int Order => 0;
    public NameFilterConfiguration Config => PluginConfig.Filters.NameFilter;

    public FilterManager? FilterManager { get; set; }

    public void Reload()
    {
    }

    public void Reset()
    {
        Config.CurrentName = "";
    }

    public bool HasValue()
    {
        return Config.CurrentName != "";
    }

    public void Set(dynamic value)
    {
        Config.CurrentName = (string)value;
        PluginConfig.Save();
    }

    public void Draw()
    {
        using var id = ImRaii.PushId("NameFilter");

        ImGui.TableNextColumn();
        TextService.Draw("NameFilter.Label");

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(250);
        var currentName = Config.CurrentName;
        if (ImGui.InputText("##Input", ref currentName, 255))
        {
            Set(currentName);
            FilterManager!.Update();
        }
    }

    public bool Run()
    {
        if (string.IsNullOrWhiteSpace(Config.CurrentName))
            return false;

        FilterManager!.State.Leves = FilterManager.State.Leves.Where(row => row.Name.AsReadOnly().ExtractText().Contains(Config.CurrentName, StringComparison.InvariantCultureIgnoreCase));

        return true;
    }
}
