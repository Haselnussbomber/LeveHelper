using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace LeveHelper;

public class ConfigWindow : Window
{
    private readonly Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("LeveHelper Config")
    {
        Plugin = plugin;

        base.Flags |= ImGuiWindowFlags.NoSavedSettings;
        base.Flags |= ImGuiWindowFlags.NoResize;
        base.Flags |= ImGuiWindowFlags.NoMove;
        base.Flags |= ImGuiWindowFlags.NoCollapse;
        base.Flags |= ImGuiWindowFlags.AlwaysAutoResize;
    }

    public override void PreDraw()
    {
        base.Position = Plugin.PluginWindow.MyPosition + new Vector2(Plugin.PluginWindow.MySize.X, 0);
    }

    public override void Draw()
    {
        var config = Configuration.Instance;

        var notifyWantedTarget = config.NotifyWantedTarget;
        if (ImGui.Checkbox("Notify when Wanted Target is found", ref notifyWantedTarget))
        {
            config.NotifyWantedTarget = notifyWantedTarget;
            Configuration.Save();
        }

        var notifyTreasure = config.NotifyTreasure;
        if (ImGui.Checkbox("Notify when Treasure is found", ref notifyTreasure))
        {
            config.NotifyTreasure = notifyTreasure;
            Configuration.Save();
        }
    }
}
