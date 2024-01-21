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
        // Notify when Wanted Target is found
        {
            var notifyWantedTarget = Plugin.Config.NotifyWantedTarget;
            if (ImGui.Checkbox(t("Config.NotifyWantedTarget"), ref notifyWantedTarget))
            {
                Plugin.Config.NotifyWantedTarget = notifyWantedTarget;
                Plugin.Config.Save();
            }
        }

        // Notify when Treasure is found
        {
            var notifyTreasure = Plugin.Config.NotifyTreasure;
            if (ImGui.Checkbox(t("Config.NotifyTreasure"), ref notifyTreasure))
            {
                Plugin.Config.NotifyTreasure = notifyTreasure;
                Plugin.Config.Save();
            }
        }
    }
}
