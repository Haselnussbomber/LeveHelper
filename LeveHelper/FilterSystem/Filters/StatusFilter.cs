using System.Linq;
using Dalamud.Interface.Utility.Raii;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Config;
using LeveHelper.Interfaces;

namespace LeveHelper.Filters;

public enum CompletedStatus
{
    Any,
    Complete,
    Incomplete,
    Accepted
}

public class StatusFilterConfiguration
{
    public CompletedStatus SelectedStatus = CompletedStatus.Any;
}

public class StatusFilter(
    PluginConfig PluginConfig,
    TextService TextService,
    LeveService LeveService) : IFilter
{
    public int Order => 1;
    public StatusFilterConfiguration Config => PluginConfig.Filters.StatusFilter;

    public FilterManager? FilterManager { get; set; }

    public void Reload()
    {
    }

    public void Reset()
    {
        Config.SelectedStatus = CompletedStatus.Any;
    }

    public bool HasValue()
    {
        return Config.SelectedStatus != CompletedStatus.Any;
    }

    public void Set(dynamic value)
    {
        Config.SelectedStatus = (CompletedStatus)value;
        PluginConfig.Save();
    }

    public void Draw()
    {
        using var id = ImRaii.PushId("StatusFilter");

        ImGui.TableNextColumn();
        TextService.Draw("StatusFilter.Label");

        ImGui.TableNextColumn();
        var values = Enum.GetValues<CompletedStatus>();

        ImGui.SetNextItemWidth(250);
        using var combo = ImRaii.Combo("##Combo", TextService.Translate("StatusFilter.Status." + Enum.GetName(typeof(CompletedStatus), Config.SelectedStatus)));
        if (!combo.Success)
            return;

        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var radio = Config.SelectedStatus == value;
            if (ImGui.Selectable(TextService.Translate("StatusFilter.Status." + Enum.GetName(typeof(CompletedStatus), value)) + $"##Entry_{i}", radio))
            {
                Set(value);
                FilterManager!.Update();
            }

            if (Config.SelectedStatus == value)
            {
                ImGui.SetItemDefaultFocus();
            }
        }
    }

    public bool Run()
    {
        if (Config.SelectedStatus == CompletedStatus.Complete)
        {
            FilterManager!.State.Leves = FilterManager.State.Leves.Where(LeveService.IsComplete);
            return true;
        }
        else if (Config.SelectedStatus == CompletedStatus.Incomplete)
        {
            FilterManager!.State.Leves = FilterManager.State.Leves.Where(leve => !LeveService.IsComplete(leve));
            return true;
        }
        else if (Config.SelectedStatus == CompletedStatus.Accepted)
        {
            FilterManager!.State.Leves = FilterManager.State.Leves.Where(LeveService.IsAccepted);
            return true;
        }

        return false;
    }
}
