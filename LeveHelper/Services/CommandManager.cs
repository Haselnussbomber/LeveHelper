using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using LeveHelper.Windows;

namespace LeveHelper.Services;

[RegisterSingleton]
public class CommandManager : IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly WindowManager _windowManager;
    private readonly CommandService _commandService;
    private readonly IClientState _clientState;
    private bool _mainUiHandlerRegistered;

    public CommandManager(
        IDalamudPluginInterface pluginInterface,
        WindowManager windowManager,
        CommandService commandService,
        IClientState clientState)
    {
        _pluginInterface = pluginInterface;
        _windowManager = windowManager;
        _commandService = commandService;
        _clientState = clientState;

        _commandService.Register("/levehelper", "CommandHandlerHelpMessage", HandleCommand, autoEnable: true);
        _commandService.Register("/lh", "CommandHandlerHelpMessage", HandleCommand, autoEnable: true);

        _pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        if (_clientState.IsLoggedIn) EnableMainUiHandler();

        _clientState.Login += OnLogin;
        _clientState.Logout += OnLogout;
    }

    private void OnLogin()
    {
        EnableMainUiHandler();
    }

    private void OnLogout(int type, int code)
    {
        DisableMainUiHandler();
    }

    public void Dispose()
    {
        DisableMainUiHandler();

        _pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;

        GC.SuppressFinalize(this);
    }

    private void EnableMainUiHandler()
    {
        if (!_mainUiHandlerRegistered)
        {
            _pluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
            _mainUiHandlerRegistered = true;
        }
    }

    private void DisableMainUiHandler()
    {
        if (_mainUiHandlerRegistered)
        {
            _pluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
            _mainUiHandlerRegistered = false;
        }
    }

    private void HandleCommand(string command, string arguments)
    {
        switch (arguments)
        {
            case "config":
                ToggleConfigWindow();
                break;

            default:
                ToggleMainWindow();
                break;
        }
    }

    private void ToggleMainWindow()
    {
        _windowManager.CreateOrOpen(Service.Get<MainWindow>).IsOpen = true;
    }

    private void ToggleConfigWindow()
    {
        _windowManager.CreateOrOpen(Service.Get<ConfigWindow>).IsOpen = true;
    }
}
