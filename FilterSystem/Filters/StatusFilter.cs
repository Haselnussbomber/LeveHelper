using System;
using System.Linq;
using ImGuiNET;

namespace LeveHelper.Filters;

public enum CompletedStatus
{
    Any,
    Complete,
    Incomplete
}

public class StatusFilter : Filter
{
    public StatusFilter(FilterManager manager) : base(manager)
    {
    }

    public Configuration Config => Service.Config.Filters.StatusFilter;

    public class Configuration
    {
        public CompletedStatus SelectedStatus = CompletedStatus.Any;
    }

    public override void Reset()
    {
        Config.SelectedStatus = CompletedStatus.Any;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedStatus = (CompletedStatus)value;
        Service.Config.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Status:");

        ImGui.TableNextColumn();
        var values = Enum.GetValues<CompletedStatus>();

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var radio = Config.SelectedStatus == value;
            if (ImGui.RadioButton(Enum.GetName(typeof(CompletedStatus), value) + $"##LeveHelper_StatusFilter_Radio-{i}", radio))
            {
                Set(value);
                manager.Update();
            }

            if (i < values.Length)
                ImGui.SameLine();
        }
    }

    public override bool Run()
    {
        if (Config.SelectedStatus == CompletedStatus.Complete)
        {
            state.leves = state.leves.Where(item => item.IsComplete);
            return true;
        }
        else if (Config.SelectedStatus == CompletedStatus.Incomplete)
        {
            state.leves = state.leves.Where(item => !item.IsComplete);
            return true;
        }

        return false;
    }
}
