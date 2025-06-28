using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Extensions;
using LeveHelper.Config;
using LeveHelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeveHelper;

public sealed class Plugin : IDalamudPlugin
{
    private readonly ServiceProvider _serviceProvider;

    public Plugin(IDalamudPluginInterface pluginInterface, IFramework framework)
    {
        _serviceProvider = new ServiceCollection()
            .AddDalamud(pluginInterface)
            .AddSingleton(PluginConfig.Load)
            .AddHaselCommon()
            .AddLeveHelper()
            .BuildServiceProvider();

        framework.RunOnFrameworkThread(() =>
        {
            _serviceProvider.GetRequiredService<CommandManager>();
            _serviceProvider.GetRequiredService<WantedTargetScanner>();
        });
    }

    void IDisposable.Dispose()
    {
        _serviceProvider.Dispose();
    }
}
