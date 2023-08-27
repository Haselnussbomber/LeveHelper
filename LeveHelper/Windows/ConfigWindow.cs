using System.Numerics;
using Dalamud.Interface.Raii;
using Dalamud.Interface.Windowing;
using HaselCommon.Enums;
using HaselCommon.Extensions;
using HaselCommon.Services;
using HaselCommon.Utils;
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

            using var popup = ImRaii.ContextPopupItem("##ChangeLanguagePopup");
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
