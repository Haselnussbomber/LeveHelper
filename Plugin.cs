using System;
using Dalamud.Plugin;

namespace LeveHelper;

public unsafe partial class Plugin : IDalamudPlugin
{
    public string Name => "LeveHelper";

    internal DalamudPluginInterface PluginInterface;
    internal PluginUi Ui;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;

        pluginInterface.Create<Service>();
        Service.Config = Configuration.Load(this);

        Ui = new(this);
    }
}

public sealed partial class Plugin : IDisposable
{
    private bool isDisposed;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (isDisposed)
            return;

        if (disposing)
        {
            Ui.Show = false;
            Ui.Dispose();
        }

        isDisposed = true;
    }
}
