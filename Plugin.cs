using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using LeveHelper.Interfaces;
using LeveHelper.Structs;
using LeveHelper.Windows;

namespace LeveHelper;

public unsafe class Plugin : IDalamudPlugin, IDisposable
{
    public string Name => "LeveHelper";

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
        HaselAtkComponentTextInput.Addresses.TriggerRedraw.Value = (nuint)Service.SigScanner.ScanText(HaselAtkComponentTextInput.Addresses.TriggerRedraw.String);
        AddonItemSearch.Addresses.SetModeFilter.Value = (nuint)Service.SigScanner.ScanText(AddonItemSearch.Addresses.SetModeFilter.String);
        AddonItemSearch.Addresses.RunSearch.Value = (nuint)Service.SigScanner.ScanText(AddonItemSearch.Addresses.RunSearch.String);

        Config = Configuration.Load();
        Service.TranslationManager.Initialize("LeveHelper.Translations.json", Config);
        Service.TranslationManager.OnLanguageChange += OnLanguageChange;

        FilterManager = new();

        Service.ClientState.Login += ClientState_Login;
        Service.ClientState.Logout += ClientState_Logout;
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

    private void ClientState_Login(object? sender, EventArgs e) => SubscribeOpenMainUi();
    private void ClientState_Logout(object? sender, EventArgs e) => UnsubscribeOpenMainUi();

    private void OnLanguageChange()
    {
        Service.StringManager.Clear();
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
        Service.ClientState.Login -= ClientState_Login;
        Service.ClientState.Logout -= ClientState_Logout;

        Service.TranslationManager.OnLanguageChange -= OnLanguageChange;

        Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigWindow;
        Service.PluginInterface.UiBuilder.OpenMainUi -= OpenMainWindow;

        Service.CommandManager.RemoveHandler("/levehelper");
        Service.CommandManager.RemoveHandler("/lh");

        Config.Save();

        Service.Dispose();
    }
}
