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
        Service.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
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

    private void OnCommand(string command, string arguments)
    {
        switch (arguments.ToLower())
        {
            case "config":
                Service.WindowManager.ToggleWindow<ConfigWindow>();
                break;

            default:
                Service.WindowManager.ToggleWindow<MainWindow>();
                break;
        }
    }

    private void SubscribeOpenMainUi()
    {
        if (!IsOpenMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
            IsOpenMainUiSubscribed = true;
        }
    }

    private void UnsubscribeOpenMainUi()
    {
        if (IsOpenMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
            IsOpenMainUiSubscribed = false;
        }
    }

    private void ToggleMainWindow() => Service.WindowManager.ToggleWindow<MainWindow>();
    private void ToggleConfigWindow() => Service.WindowManager.ToggleWindow<ConfigWindow>();

    void IDisposable.Dispose()
    {
        Service.ClientState.Login -= SubscribeOpenMainUi;
        Service.ClientState.Logout -= UnsubscribeOpenMainUi;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;
        Service.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
        Service.PluginInterface.LanguageChanged -= PluginInterface_LanguageChanged;

        Service.CommandManager.RemoveHandler("/levehelper");
        Service.CommandManager.RemoveHandler("/lh");

        Service.Dispose();
    }
}
