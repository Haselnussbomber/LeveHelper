using System.Threading;
using System.Threading.Tasks;
using AutoCtor;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using HaselCommon.Services.Commands;
using LeveHelper.Windows;
using Microsoft.Extensions.Hosting;

namespace LeveHelper.Services;

[RegisterSingleton<IHostedService>(Duplicate = DuplicateStrategy.Append), AutoConstruct]
public partial class PluginCommandService : IHostedService
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly WindowManager _windowManager;
    private readonly CommandService _commandService;
    private readonly IClientState _clientState;
    private bool _mainUiHandlerRegistered;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _commandService.AddCommand("levehelper", cmd => cmd
            .WithHelpTextKey("LeveHelper.CommandHandlerHelpMessage")
            .WithHandler(OnMainCommand)
            .AddSubcommand("config", cmd => cmd
                .WithHelpTextKey("LeveHelper.CommandHandler.Config.HelpMessage")
                .WithHandler(OnConfigCommand)));

        // TODO: add command aliases lol
        _commandService.AddCommand("lh", cmd => cmd
            .WithHelpTextKey("LeveHelper.CommandHandlerHelpMessage")
            .WithHandler(OnMainCommand)
            .AddSubcommand("config", cmd => cmd
                .WithHelpTextKey("LeveHelper.CommandHandler.Config.HelpMessage")
                .WithHandler(OnConfigCommand)));

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

    private void OnMainCommand(CommandContext ctx)
    {
        ToggleMainWindow();
    }

    private void OnConfigCommand(CommandContext ctx)
    {
        ToggleConfigWindow();
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
