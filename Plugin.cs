using System;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;

namespace LeveHelper;

public class Plugin : IDalamudPlugin, IDisposable
{
    public string Name => "LeveHelper";

    internal static WindowSystem WindowSystem = new("LeveHelper");
    internal static PluginWindow PluginWindow = null!;
    internal static ConfigWindow ConfigWindow = null!;
    internal static CraftingHelperWindow? CraftingHelperWindow;

    internal static Configuration Config = null!;
    internal static FilterManager FilterManager = null!;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Service.GameFunctions = new();

        Config = Configuration.Load();
        PlaceNameHelper.Connect();
        Scanner.Connect();

        WindowSystem.AddWindow(PluginWindow = new PluginWindow());
        WindowSystem.AddWindow(ConfigWindow = new ConfigWindow());

        Service.PluginInterface.UiBuilder.Draw += OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        var commandInfo = new CommandInfo(OnCommand)
        {
            HelpMessage = "Show Window"
        };

        Service.Commands.AddHandler("/levehelper", commandInfo);
        Service.Commands.AddHandler("/lh", commandInfo);

        Service.Framework.RunOnFrameworkThread(() =>
        {
            Task.Run(GatheringPointCache.Load);
        });
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
        else if (args.ToLowerInvariant() == "c")
        {
            if (CraftingHelperWindow == null)
            {
                CraftingHelperWindow = new CraftingHelperWindow();
                WindowSystem.AddWindow(CraftingHelperWindow);
            }

            CraftingHelperWindow.IsOpen = true;
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

        Config.Save();
        PlaceNameHelper.Disconnect();
        Scanner.Disconnect();

        WindowSystem = null!;
        PluginWindow = null!;
        ConfigWindow = null!;
        CraftingHelperWindow = null!;
        FilterManager = null!;
        Config = null!;
    }
}
