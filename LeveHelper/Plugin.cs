using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon;
using LeveHelper.Config;
using LeveHelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeveHelper;

public sealed class Plugin : IDalamudPlugin
{
    public Plugin(IDalamudPluginInterface pluginInterface, IFramework framework)
    {
        Service.Collection
            .AddDalamud(pluginInterface)
            .AddSingleton(PluginConfig.Load)
            .AddHaselCommon()
            .AddLeveHelper();

        Service.BuildProvider();

        framework.RunOnFrameworkThread(() =>
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
