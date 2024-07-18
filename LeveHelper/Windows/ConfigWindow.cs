using HaselCommon.Services;
using HaselCommon.Utils;
using HaselCommon.Windowing;
using ImGuiNET;
using LeveHelper.Config;

namespace LeveHelper.Windows;

public class ConfigWindow : SimpleWindow
{
    private readonly TextService TextService;
    private readonly PluginConfig PluginConfig;

    public ConfigWindow(
        WindowManager windowManager,
        TextService textService,
        PluginConfig pluginConfig)
        : base(windowManager, textService.Translate("WindowTitle.Configuration"))
    {
        TextService = textService;
        PluginConfig = pluginConfig;

        AllowClickthrough = false;
        AllowPinning = false;
        Flags |= ImGuiWindowFlags.AlwaysAutoResize;
    }

    public override void Draw()
    {
        // Notify when Wanted Target is found
        {
            var notifyWantedTarget = PluginConfig.NotifyWantedTarget;
            if (ImGui.Checkbox(TextService.Translate("Config.NotifyWantedTarget"), ref notifyWantedTarget))
            {
                PluginConfig.NotifyWantedTarget = notifyWantedTarget;
                PluginConfig.Save();
            }
        }

        // Notify when Treasure is found
        {
            var notifyTreasure = PluginConfig.NotifyTreasure;
            if (ImGui.Checkbox(TextService.Translate("Config.NotifyTreasure"), ref notifyTreasure))
            {
                PluginConfig.NotifyTreasure = notifyTreasure;
                PluginConfig.Save();
            }
        }

        // Show button to import the crafting list to TeamCraft
        {
            var showImportOnTeamCraftButton = PluginConfig.ShowImportOnTeamCraftButton;
            if (ImGui.Checkbox(TextService.Translate("Config.ShowImportOnTeamCraftButton.Label"), ref showImportOnTeamCraftButton))
            {
                PluginConfig.ShowImportOnTeamCraftButton = showImportOnTeamCraftButton;
                PluginConfig.Save();
            }
            using (ImGuiUtils.ConfigIndent())
                TextService.Draw(Colors.Grey, "Config.ShowImportOnTeamCraftButton.Description");
        }
    }
}
