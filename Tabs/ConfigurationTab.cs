using ImGuiNET;

namespace LeveHelper;

public class ConfigurationTab
{
    public PluginWindow Window { get; }

    public ConfigurationTab(PluginWindow window)
    {
        Window = window;
    }

    public void Draw()
    {
        var config = Plugin.Config;

        // Notify when Wanted Target is found
        {
            var notifyWantedTarget = config.NotifyWantedTarget;
            if (ImGui.Checkbox("Notify when Wanted Target is found", ref notifyWantedTarget))
            {
                config.NotifyWantedTarget = notifyWantedTarget;
                config.Save();
            }
        }

        // Notify when Treasure is found
        {
            var notifyTreasure = config.NotifyTreasure;
            if (ImGui.Checkbox("Notify when Treasure is found", ref notifyTreasure))
            {
                config.NotifyTreasure = notifyTreasure;
                config.Save();
            }
        }

        // Show Recipe Tree in Leve list
        {
            var showInlineRecipeTree = config.ShowInlineRecipeTree;
            if (ImGui.Checkbox("Show Recipe Tree in Leve list", ref showInlineRecipeTree))
            {
                config.ShowInlineRecipeTree = showInlineRecipeTree;
                config.Save();
            };
        }

        // Show for accepted leves only
        {
            ImGui.Indent();
            var showInlineRecipeTreeForAcceptedOnly = config.ShowInlineRecipeTreeForAcceptedOnly;

            if (!config.ShowInlineRecipeTree)
                ImGui.BeginDisabled();

            if (ImGui.Checkbox("Show for accepted leves only", ref showInlineRecipeTreeForAcceptedOnly))
            {
                config.ShowInlineRecipeTreeForAcceptedOnly = showInlineRecipeTreeForAcceptedOnly;
                config.Save();
            }

            if (!config.ShowInlineRecipeTree)
                ImGui.EndDisabled();

            ImGui.Unindent();
        }

        // Show result item only
        {
            ImGui.Indent();
            var showInlineResultItemOnly = config.ShowInlineResultItemOnly;

            if (!config.ShowInlineRecipeTree)
                ImGui.BeginDisabled();

            if (ImGui.Checkbox("Show result item only", ref showInlineResultItemOnly))
            {
                config.ShowInlineResultItemOnly = showInlineResultItemOnly;
                config.Save();
            }

            if (!config.ShowInlineRecipeTree)
                ImGui.EndDisabled();

            ImGui.Unindent();
        }
    }
}
