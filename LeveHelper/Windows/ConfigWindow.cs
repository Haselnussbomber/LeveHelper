using AutoCtor;
using HaselCommon.Graphics;
using HaselCommon.Gui;
using HaselCommon.Services;
using LeveHelper.Config;

namespace LeveHelper.Windows;

[RegisterTransient, AutoConstruct]
public partial class ConfigWindow : SimpleWindow
{
    private readonly TextService _textService;
    private readonly PluginConfig _config;

    [AutoPostConstruct]
    private void Initialize()
    {
        AllowClickthrough = false;
        AllowPinning = false;
        Flags |= ImGuiWindowFlags.AlwaysAutoResize;
    }

    public override void Draw()
    {
        // Notify when Wanted Target is found
        {
            var notifyWantedTarget = _config.NotifyWantedTarget;
            if (ImGui.Checkbox(_textService.Translate("Config.NotifyWantedTarget"), ref notifyWantedTarget))
            {
                _config.NotifyWantedTarget = notifyWantedTarget;
                _config.Save();
            }
        }

        // Notify when Treasure is found
        {
            var notifyTreasure = _config.NotifyTreasure;
            if (ImGui.Checkbox(_textService.Translate("Config.NotifyTreasure"), ref notifyTreasure))
            {
                _config.NotifyTreasure = notifyTreasure;
                _config.Save();
            }
        }

        // Show button to import the crafting list to TeamCraft
        {
            var showImportOnTeamCraftButton = _config.ShowImportOnTeamCraftButton;
            if (ImGui.Checkbox(_textService.Translate("Config.ShowImportOnTeamCraftButton.Label"), ref showImportOnTeamCraftButton))
            {
                _config.ShowImportOnTeamCraftButton = showImportOnTeamCraftButton;
                _config.Save();
            }
            using (ImGuiUtils.ConfigIndent())
            using (Color.Grey.Push(ImGuiCol.Text))
                ImGui.TextUnformatted(_textService.Translate("Config.ShowImportOnTeamCraftButton.Description"));

            ImGui.Spacing();
        }
    }
}
