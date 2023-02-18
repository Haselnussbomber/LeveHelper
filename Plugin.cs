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

    internal static Configuration Config = null!;
    internal static FilterManager FilterManager = null!;
    internal static byte StartTown;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        FFXIVClientStructs.Interop.Resolver.GetInstance.SetupSearchSpace(Service.SigScanner.SearchBase);
        FFXIVClientStructs.Interop.Resolver.GetInstance.Resolve();
        Service.GameFunctions = new();

        Config = Configuration.Load();
        PlaceNameHelper.Connect();
        Scanner.Connect();

        Service.Framework.RunOnFrameworkThread(() =>
        {
            FilterManager = new();

            WindowSystem.AddWindow(PluginWindow = new PluginWindow());

            Service.PluginInterface.UiBuilder.Draw += OnDraw;
            Service.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

            var commandInfo = new CommandInfo(OnCommand)
            {
                HelpMessage = "Show Window"
            };

            Service.Commands.AddHandler("/levehelper", commandInfo);
            Service.Commands.AddHandler("/lh", commandInfo);

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
        PluginWindow.Toggle();
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
        FilterManager = null!;
        Config = null!;
    }
}
