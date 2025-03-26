using Dalamud.Plugin;
using HaselCommon;
using LeveHelper.Config;
using LeveHelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeveHelper;

public sealed class Plugin : IDalamudPlugin
{
    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Service.Collection
            .AddDalamud(pluginInterface)
            .AddSingleton(PluginConfig.Load)
            .AddHaselCommon()
            .AddLeveHelper();

        Service.Initialize(() =>
        {
            Service.Get<CommandManager>();
            Service.Get<WantedTargetScanner>();
        });
    }

    void IDisposable.Dispose()
    {
        Service.Dispose();
    }
}
