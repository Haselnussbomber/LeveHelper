using System;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;

namespace LeveHelper;

public class Plugin : IDalamudPlugin, IDisposable
{
    public string Name => "LeveHelper";

    private readonly WindowSystem windowSystem = new("LeveHelper");
    private readonly PluginWindow pluginWindow;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Service.GameFunctions = new();

        Configuration.Load();
        PlaceNameHelper.Connect();

        this.pluginWindow = new PluginWindow();
        this.windowSystem.AddWindow(this.pluginWindow);

        Service.PluginInterface.UiBuilder.Draw += this.OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += this.OnOpenConfigUi;

        var commandInfo = new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Show Window"
        };

        Service.Commands.AddHandler("/levehelper", commandInfo);
        Service.Commands.AddHandler("/lh", commandInfo);

#if DEBUG
        this.windowSystem.GetWindow("LeveHelper")?.Toggle();
#endif
    }

    private void OnDraw()
    {
        try
        {
            this.windowSystem.Draw();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unexpected exception in OnDraw");
        }
    }

    private void OnCommand(string command, string args)
    {
        this.pluginWindow.Toggle();
    }

    private void OnOpenConfigUi()
    {
        this.pluginWindow.Toggle();
    }

    void IDisposable.Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= this.OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= this.OnOpenConfigUi;

        Service.Commands.RemoveHandler("/levehelper");
        Service.Commands.RemoveHandler("/lh");

        this.windowSystem.RemoveAllWindows();

        Configuration.Save();
        PlaceNameHelper.Disconnect();

        ((IDisposable)Configuration.Instance).Dispose();
    }
}
