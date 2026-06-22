using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Extensions;
using LeveHelper.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeveHelper;

[AutoConstruct]
public partial class Plugin : IAsyncDalamudPlugin
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly IPluginLog _pluginLog;
    private readonly IFramework _framework;
    private IHost? _host;

    public Task LoadAsync(CancellationToken cancellationToken)
    {
        _host = new HostBuilder()
            .UseContentRoot(_pluginInterface.AssemblyLocation.Directory!.FullName)
            .ConfigureServices(services =>
            {
                services.AddDalamud(_pluginInterface);
                services.AddConfig(PluginConfig.Load(_pluginInterface, _pluginLog));
                services.AddHaselCommon();
                services.AddLeveHelper();
            })
            .Build();

        return _host.StartOnFrameworkThread(_framework, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _host?.StopOnFrameworkThread(_framework) ?? ValueTask.CompletedTask;
    }
}
