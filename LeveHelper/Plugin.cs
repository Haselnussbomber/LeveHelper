using System.IO;
using Dalamud.Game;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon;
using InteropGenerator.Runtime;
using LeveHelper.Config;
using LeveHelper.Services;
using LeveHelper.Windows;
using Microsoft.Extensions.DependencyInjection;

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
#if HAS_LOCAL_CS
        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        Resolver.GetInstance.Setup(
            sigScanner.SearchBase,
            dataManager.GameData.Repositories["ffxiv"].Version,
            new FileInfo(Path.Join(pluginInterface.ConfigDirectory.FullName, "SigCache.json")));
        Resolver.GetInstance.Resolve();
#endif

        Service.Collection
            .AddDalamud(pluginInterface)
            .AddSingleton(PluginConfig.Load)
            .AddHaselCommon()
            .AddLeveHelper();

        Service.BuildProvider();

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
