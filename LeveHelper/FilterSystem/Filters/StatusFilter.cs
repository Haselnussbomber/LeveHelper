using System.Linq;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

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

public class StatusFilter : Filter
{
    public StatusFilter(FilterManager manager) : base(manager)
    {
    }

    public static StatusFilterConfiguration Config => Plugin.Config.Filters.StatusFilter;

    public override void Reload()
    {
    }

    public override void Reset()
    {
        Config.SelectedStatus = CompletedStatus.Any;
    }

    public override bool HasValue()
    {
        return Config.SelectedStatus != CompletedStatus.Any;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedStatus = (CompletedStatus)value;
        Plugin.Config.Save();
    }

    public override void Draw()
    {
        using var id = ImRaii.PushId("StatusFilter");

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(t("StatusFilter.Label"));

        ImGui.TableNextColumn();
        var values = Enum.GetValues<CompletedStatus>();

        ImGui.SetNextItemWidth(InputWidth);
        using var combo = ImRaii.Combo("##Combo", t("StatusFilter.Status." + Enum.GetName(typeof(CompletedStatus), Config.SelectedStatus)));
        if (!combo.Success)
            return;

        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var radio = Config.SelectedStatus == value;
            if (ImGui.Selectable(t("StatusFilter.Status." + Enum.GetName(typeof(CompletedStatus), value)) + $"##Entry_{i}", radio))
            {
                Set(value);
                manager.Update();
            }

            if (Config.SelectedStatus == value)
            {
                ImGui.SetItemDefaultFocus();
            }
        }
    }

    public override bool Run()
    {
        if (Config.SelectedStatus == CompletedStatus.Complete)
        {
            state.Leves = state.Leves.Where(item => item.IsComplete);
            return true;
        }
        else if (Config.SelectedStatus == CompletedStatus.Incomplete)
        {
            state.Leves = state.Leves.Where(item => !item.IsComplete);
            return true;
        }
        else if (Config.SelectedStatus == CompletedStatus.Accepted)
        {
            state.Leves = Service.GameFunctions.ActiveLevequests;
            return true;
        }

        return false;
    }
}
