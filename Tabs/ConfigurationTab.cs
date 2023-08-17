using Dalamud.Interface.Raii;
using ImGuiNET;
using LeveHelper.Enums;
using LeveHelper.Extensions;
using LeveHelper.Services;
using LeveHelper.Utils;

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

        // Language
        {
            ImGui.Spacing();
            ImGuiUtils.PushCursorY(ImGui.GetStyle().FramePadding.Y);
            ImGui.TextUnformatted("Language:");
            ImGui.SameLine();
            ImGui.TextUnformatted(Service.TranslationManager.Language);
            ImGui.SameLine();
            ImGuiUtils.PushCursorY(-ImGui.GetStyle().FramePadding.Y);

            if (ImGui.Button("Change##ChangeLanguageButton"))
            {
                ImGui.OpenPopup("##ChangeLanguagePopup");
            }

            using var popup = ImRaii.ContextPopup("##ChangeLanguagePopup");
            if (popup.Success)
            {
                static string GetLabel(string type, string code)
                {
                    return TranslationManager.AllowedLanguages.ContainsKey(code)
                        ? $"Override: {type} ({code})"
                        : $"Override: {type} ({code} is not supported, using fallback {TranslationManager.DefaultLanguage})";
                }
                if (ImGui.MenuItem(GetLabel("Dalamud", Service.PluginInterface.UiLanguage), "", Service.TranslationManager.Override == PluginLanguageOverride.Dalamud))
                {
                    Service.TranslationManager.Override = PluginLanguageOverride.Dalamud;
                }

                if (ImGui.MenuItem(GetLabel("Client", Service.ClientState.ClientLanguage.ToCode()), "", Service.TranslationManager.Override == PluginLanguageOverride.Client))
                {
                    Service.TranslationManager.Override = PluginLanguageOverride.Client;
                }

                ImGui.Separator();

                foreach (var (code, name) in TranslationManager.AllowedLanguages)
                {
                    if (ImGui.MenuItem(name, "", Service.TranslationManager.Override == PluginLanguageOverride.None && Service.TranslationManager.Language == code))
                    {
                        Service.TranslationManager.SetLanguage(PluginLanguageOverride.None, code);
                    }
                }
            }
        }
        ImGui.Spacing();

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
