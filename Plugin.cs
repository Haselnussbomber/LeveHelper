using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using LeveHelper.Structs;

namespace LeveHelper;

public unsafe class Plugin : IDalamudPlugin, IDisposable
{
    public string Name => "LeveHelper";

    private WindowSystem _windowSystem { get; set; } = new("LeveHelper");
    private PluginWindow? _pluginWindow { get; set; }

    internal static Configuration Config { get; private set; } = null!;
    internal static FilterManager FilterManager { get; private set; } = null!;

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

        Service.PluginInterface.UiBuilder.Draw += OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += ToggleWindow;

        var commandInfo = new CommandInfo(OnCommand)
        {
            HelpMessage = "Show Window"
        };

        Service.CommandManager.AddHandler("/levehelper", commandInfo);
        Service.CommandManager.AddHandler("/lh", commandInfo);
    }

    private void OnLanguageChange()
    {
        Service.StringManager.Clear();
        FilterManager.Reload();
        _pluginWindow?.Refresh();
    }

    private void OnDraw()
    {
        try
        {
            _windowSystem.Draw();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unexpected exception in OnDraw");
        }
    }

    private void OnCommand(string command, string args)
    {
        ToggleWindow();
    }

    private void OpenWindow()
    {
        _windowSystem.AddWindow(_pluginWindow = new PluginWindow(this));
    }

    internal void CloseWindow()
    {
        if (_pluginWindow == null)
            return;

        _windowSystem.RemoveWindow(_pluginWindow);
        _pluginWindow.Dispose();
        _pluginWindow = null;
    }

    private void ToggleWindow()
    {
        if (_pluginWindow == null)
            OpenWindow();
        else
            CloseWindow();
    }

    void IDisposable.Dispose()
    {
        Service.TranslationManager.OnLanguageChange -= OnLanguageChange;
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= ToggleWindow;

        Service.CommandManager.RemoveHandler("/levehelper");
        Service.CommandManager.RemoveHandler("/lh");

        CloseWindow();
        _windowSystem.RemoveAllWindows();
        _windowSystem = null!;

        Config.Save();
        Config = null!;

        FilterManager = null!;

        Service.Dispose();
    }
}
