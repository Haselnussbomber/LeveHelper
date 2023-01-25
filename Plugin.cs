using System;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;

namespace LeveHelper;

public class Plugin : IDalamudPlugin, IDisposable
{
    public string Name => "LeveHelper";

    internal readonly WindowSystem WindowSystem = new("LeveHelper");
    internal readonly PluginWindow PluginWindow;
    internal readonly ConfigWindow ConfigWindow;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Service.GameFunctions = new();

        Configuration.Load();
        PlaceNameHelper.Connect();
        Scanner.Connect();

        WindowSystem.AddWindow(PluginWindow = new PluginWindow(this));
        WindowSystem.AddWindow(ConfigWindow = new ConfigWindow(this));

        Service.PluginInterface.UiBuilder.Draw += OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        var commandInfo = new CommandInfo(OnCommand)
        {
            HelpMessage = "Show Window"
        };

        Service.Commands.AddHandler("/levehelper", commandInfo);
        Service.Commands.AddHandler("/lh", commandInfo);
    }

    private void OnDraw()
    {
        try
        {
            WindowSystem.Draw();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unexpected exception in OnDraw");
        }
    }

    private void OnCommand(string command, string args)
    {
        if (args.ToLowerInvariant() == "config")
        {
            ConfigWindow.Toggle();
        }
        else
        {
            PluginWindow.Toggle();
        }
    }

    private void OnOpenConfigUi()
    {
        PluginWindow.Toggle();
    }

    void IDisposable.Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;

        Service.Commands.RemoveHandler("/levehelper");
        Service.Commands.RemoveHandler("/lh");

        WindowSystem.RemoveAllWindows();

        Configuration.Save();
        PlaceNameHelper.Disconnect();
        Scanner.Disconnect();

        ((IDisposable)Configuration.Instance).Dispose();
    }
}
