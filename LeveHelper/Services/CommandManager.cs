using AutoCtor;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using LeveHelper.Windows;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public partial class CommandManager : IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly WindowManager _windowManager;
    private readonly CommandService _commandService;
    private readonly IClientState _clientState;
    private bool _mainUiHandlerRegistered;

    [AutoPostConstruct]
    private void Initialize()
    {
        _commandService.Register("/levehelper", "CommandHandlerHelpMessage", HandleCommand, autoEnable: true);
        _commandService.Register("/lh", "CommandHandlerHelpMessage", HandleCommand, autoEnable: true);

        _pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        if (_clientState.IsLoggedIn) EnableMainUiHandler();

        _clientState.Login += OnLogin;
        _clientState.Logout += OnLogout;
    }

    public void Dispose()
    {
        DisableMainUiHandler();

        _clientState.Login -= OnLogin;
        _clientState.Logout -= OnLogout;

        _pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;

        GC.SuppressFinalize(this);
    }

    private void OnLogin()
    {
        EnableMainUiHandler();
    }

    private void OnLogout(int type, int code)
    {
        DisableMainUiHandler();
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
        _windowManager.CreateOrToggle(Service.Get<MainWindow>);
    }

    private void ToggleConfigWindow()
    {
        _windowManager.CreateOrToggle(Service.Get<ConfigWindow>);
    }
}
