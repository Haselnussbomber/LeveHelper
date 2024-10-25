using Dalamud.Game;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Extensions.DependencyInjection;
using HaselCommon.Logger;
using LeveHelper.Caches;
using LeveHelper.Config;
using LeveHelper.Interfaces;
using LeveHelper.Records;
using LeveHelper.Services;
using LeveHelper.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LeveHelper;

public sealed class Plugin : IDalamudPlugin
{
    public Plugin(
        IDalamudPluginInterface pluginInterface,
        IFramework framework,
        IPluginLog pluginLog,
        ISigScanner sigScanner,
        IDataManager dataManager)
    {
        Service
            // Dalamud & HaselCommon
            .Initialize(pluginInterface, pluginLog)

            // Logging
            .AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddProvider(new DalamudLoggerProvider(pluginLog));
            })

            // Config
            .AddSingleton(PluginConfig.Load(pluginInterface, pluginLog))

            // LeveHelper
            .AddSingleton<ExtendedItemService>()
            .AddIServices<IFilter>()
            .AddSingleton<FiltersState>()
            .AddSingleton<FilterManager>()
            .AddSingleton<WindowState>()
            .AddSingleton<WantedTargetScanner>()
            .AddSingleton<LeveIssuerCache>()
            .AddSingleton<LeveRequiredItemsCache>()
            .AddSingleton<DebugTab>()
            .AddSingleton<ListTab>()
            .AddSingleton<QueueTab>()
            .AddSingleton<RecipeTreeTab>()

            // Windows
            .AddSingleton<MainWindow>()
            .AddSingleton<ConfigWindow>();

        Service.BuildProvider();

#if HAS_LOCAL_CS
        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        Resolver.GetInstance.Setup(
            sigScanner.SearchBase,
            dataManager.GameData.Repositories["ffxiv"].Version,
            new FileInfo(Path.Join(pluginInterface.ConfigDirectory.FullName, "SigCache.json")));
        Resolver.GetInstance.Resolve();
#endif

        // TODO: IHostedService?
        framework.RunOnFrameworkThread(() =>
        {
            Service.Get<WantedTargetScanner>();
            Service.Get<MainWindow>();
        });
    }

    void IDisposable.Dispose()
    {
        Service.Dispose();
    }
}
