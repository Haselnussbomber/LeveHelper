using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using LeveHelper.Interfaces;
using LeveHelper.Windows;

namespace LeveHelper;

public unsafe class Plugin : IDalamudPlugin, IDisposable
{
    internal static Configuration Config { get; private set; } = null!;
    internal static FilterManager FilterManager { get; private set; } = null!;

    private bool _openMainUiSubscribed = false;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Task.Run(Setup);
    }

    private void Setup()
    {
        Config = Configuration.Load();

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LeveHelper.Translations.json")
            ?? throw new Exception($"Could not find translations resource \"LeveHelper.Translations.json\".");

        var translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(stream) ?? new();

        Service.TranslationManager.Initialize(translations, Config);
        Service.TranslationManager.OnLanguageChange += OnLanguageChange;

        FilterManager = new();

        Service.ClientState.Login += SubscribeOpenMainUi;
        Service.ClientState.Logout += UnsubscribeOpenMainUi;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OpenConfigWindow;

        var commandInfo = new CommandInfo(OnCommand)
        {
            HelpMessage = "Show Window"
        };

        Service.CommandManager.AddHandler("/levehelper", commandInfo);
        Service.CommandManager.AddHandler("/lh", commandInfo);

        if (Service.ClientState.IsLoggedIn)
            SubscribeOpenMainUi();
    }

    private void OnLanguageChange()
    {
        FilterManager.Reload();

        foreach (var window in Service.WindowManager.Windows)
        {
            (window as IPluginWindow)?.OnLanguageChange();
        }
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
        if (!_openMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi += OpenMainWindow;
            _openMainUiSubscribed = true;
        }
    }

    private void UnsubscribeOpenMainUi()
    {
        if (_openMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi -= OpenMainWindow;
            _openMainUiSubscribed = false;
        }
    }

    private void OpenMainWindow() => Service.WindowManager.OpenWindow<MainWindow>();
    private void OpenConfigWindow() => Service.WindowManager.OpenWindow<ConfigWindow>();

    void IDisposable.Dispose()
    {
        Service.ClientState.Login -= SubscribeOpenMainUi;
        Service.ClientState.Logout -= UnsubscribeOpenMainUi;

        Service.TranslationManager.OnLanguageChange -= OnLanguageChange;

        Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigWindow;
        Service.PluginInterface.UiBuilder.OpenMainUi -= OpenMainWindow;

        Service.CommandManager.RemoveHandler("/levehelper");
        Service.CommandManager.RemoveHandler("/lh");

        Config.Save();
        Config = null!;
        FilterManager = null!;

        Service.Dispose();
    }
}
