using Dalamud.Interface.Raii;
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
        using var windowId = ImRaii.PushId("##ConfigurationTab");

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
    }
}
