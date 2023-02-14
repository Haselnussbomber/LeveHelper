using System;
using System.Linq;
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
        ImGui.TableNextColumn();
        ImGui.Text("Status:");

        ImGui.TableNextColumn();
        var values = Enum.GetValues<CompletedStatus>();

        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var radio = Config.SelectedStatus == value;
            if (ImGui.RadioButton(Enum.GetName(typeof(CompletedStatus), value) + $"##LeveHelper_StatusFilter_Radio-{i}", radio))
            {
                Set(value);
                manager.Update();
            }

            if (i < values.Length)
            {
                ImGui.SameLine();
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
