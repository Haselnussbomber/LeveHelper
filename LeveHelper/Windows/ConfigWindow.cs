using System.Reflection;
using System.Threading.Tasks;
using AutoCtor;
using HaselCommon.Extensions;
using HaselCommon.Graphics;
using HaselCommon.Gui;
using HaselCommon.Services;
using HaselCommon.Windows;
using LeveHelper.Config;

namespace LeveHelper.Windows;

[RegisterTransient, AutoConstruct]
public partial class ConfigWindow : SimpleWindow
{
    private readonly IServiceProvider _serviceProvider;
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
                ImGui.Text(_textService.Translate("Config.ShowImportOnTeamCraftButton.Description"));

            ImGui.Spacing();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        var cursorPos = ImGui.GetCursorPos();
        var contentAvail = ImGui.GetContentRegionAvail();

        ImGuiUtils.DrawLink("GitHub", _textService.Translate("ConfigWindow.GitHubLink.Tooltip"), "https://github.com/Haselnussbomber/LeveHelper");
        ImGui.SameLine();
        ImGui.Text("•");
        ImGui.SameLine();
        ImGuiUtils.DrawLink("Ko-fi", _textService.Translate("ConfigWindow.KoFiLink.Tooltip"), "https://ko-fi.com/haselnussbomber");
        ImGui.SameLine();
        ImGui.Text("•");
        ImGui.SameLine();
        ImGui.Text(_textService.Translate("ConfigWindow.Licenses"));
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && _serviceProvider.TryGetService<LicensesWindow>(out var licensesWindow))
            {
                Task.Run(licensesWindow.Toggle);
            }
        }

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        if (version != null)
        {
            var versionString = "v" + version.ToString(3);
            ImGui.SetCursorPos(cursorPos + contentAvail - ImGui.CalcTextSize(versionString));
            ImGuiUtils.TextUnformattedDisabled(versionString);
        }
    }
}
