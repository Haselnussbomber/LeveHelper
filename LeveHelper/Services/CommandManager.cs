using System.Threading;
using System.Threading.Tasks;
using AutoCtor;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using LeveHelper.Windows;
using Microsoft.Extensions.Hosting;

namespace LeveHelper.Services;

[RegisterSingleton<IHostedService>(Duplicate = DuplicateStrategy.Append), AutoConstruct]
public partial class CommandManager : IHostedService
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly WindowManager _windowManager;
    private readonly CommandService _commandService;
    private readonly IClientState _clientState;
    private bool _mainUiHandlerRegistered;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _commandService.Register("/levehelper", "CommandHandlerHelpMessage", HandleCommand, autoEnable: true);
        _commandService.Register("/lh", "CommandHandlerHelpMessage", HandleCommand, autoEnable: true);

        _pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        if (_clientState.IsLoggedIn) EnableMainUiHandler();

        _clientState.Login += OnLogin;
        _clientState.Logout += OnLogout;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        DisableMainUiHandler();

        _clientState.Login -= OnLogin;
        _clientState.Logout -= OnLogout;

        _pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;

        return Task.CompletedTask;
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
        _windowManager.CreateOrToggle<MainWindow>();
    }

    private void ToggleConfigWindow()
    {
        _windowManager.CreateOrToggle<ConfigWindow>();
    }
}
