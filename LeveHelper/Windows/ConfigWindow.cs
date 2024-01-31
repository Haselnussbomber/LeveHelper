using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace LeveHelper.Windows;

public unsafe class ConfigWindow : Window
{
    public ConfigWindow() : base(t("WindowTitle.Configuration"))
    {
        Namespace = "LeveHelperConfig";

        Size = new Vector2(400, 400);
        SizeCondition = ImGuiCond.Appearing;
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(4096, 2160)
        };
    }

    public override void OnClose()
    {
        Service.WindowManager.CloseWindow<ConfigWindow>();
    }

    public override void Draw()
    {
        var config = Service.GetService<Configuration>();

        // Notify when Wanted Target is found
        {
            var notifyWantedTarget = config.NotifyWantedTarget;
            if (ImGui.Checkbox(t("Config.NotifyWantedTarget"), ref notifyWantedTarget))
            {
                config.NotifyWantedTarget = notifyWantedTarget;
                config.Save();
            }
        }

        // Notify when Treasure is found
        {
            var notifyTreasure = config.NotifyTreasure;
            if (ImGui.Checkbox(t("Config.NotifyTreasure"), ref notifyTreasure))
            {
                config.NotifyTreasure = notifyTreasure;
                config.Save();
            }
        }
    }
}
