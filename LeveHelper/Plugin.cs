using Dalamud.Game.Command;
using Dalamud.Plugin;
using LeveHelper.Services;
using LeveHelper.Windows;

namespace LeveHelper;

public sealed class Plugin : IDalamudPlugin
{
    private readonly CommandInfo CommandInfo;
    private bool IsOpenMainUiSubscribed = false;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.AddService(Configuration.Load());
        Service.AddService<WantedTargetScanner>();
        Service.AddService<FilterManager>();

        Service.ClientState.Login += SubscribeOpenMainUi;
        Service.ClientState.Logout += UnsubscribeOpenMainUi;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OpenConfigWindow;
        Service.PluginInterface.LanguageChanged += PluginInterface_LanguageChanged;

        CommandInfo = new CommandInfo(OnCommand) { HelpMessage = t("CommandHandlerHelpMessage") };

        Service.CommandManager.AddHandler("/levehelper", CommandInfo);
        Service.CommandManager.AddHandler("/lh", CommandInfo);

        if (Service.ClientState.IsLoggedIn)
            SubscribeOpenMainUi();
    }

    private void PluginInterface_LanguageChanged(string langCode)
    {
        CommandInfo.HelpMessage = t("CommandHandlerHelpMessage");

        Service.GetService<FilterManager>().Reload();
        Service.WindowManager.GetWindow<MainWindow>()?.OnLanguageChange();
    }

    private void OnCommand(string command, string args)
    {
        if (args == "config")
            Service.WindowManager.ToggleWindow<ConfigWindow>();
        else
            Service.WindowManager.ToggleWindow<MainWindow>();
    }

    private void SubscribeOpenMainUi()
    {
        if (!IsOpenMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi += OpenMainWindow;
            IsOpenMainUiSubscribed = true;
        }
    }

    private void UnsubscribeOpenMainUi()
    {
        if (IsOpenMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi -= OpenMainWindow;
            IsOpenMainUiSubscribed = false;
        }
    }

    private void OpenMainWindow() => Service.WindowManager.OpenWindow<MainWindow>();
    private void OpenConfigWindow() => Service.WindowManager.OpenWindow<ConfigWindow>();

    void IDisposable.Dispose()
    {
        Service.ClientState.Login -= SubscribeOpenMainUi;
        Service.ClientState.Logout -= UnsubscribeOpenMainUi;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigWindow;
        Service.PluginInterface.UiBuilder.OpenMainUi -= OpenMainWindow;
        Service.PluginInterface.LanguageChanged -= PluginInterface_LanguageChanged;

        Service.CommandManager.RemoveHandler("/levehelper");
        Service.CommandManager.RemoveHandler("/lh");

        Service.Dispose();
    }
}
