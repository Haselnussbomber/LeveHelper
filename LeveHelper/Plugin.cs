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
    private readonly IFramework _framework;
    private IHost _host;

    [AutoPostConstruct]
    private void Initialize()
    {
        _host = new HostBuilder()
            .UseContentRoot(_pluginInterface.AssemblyLocation.Directory!.FullName)
            .ConfigureServices(services =>
            {
                services.AddDalamud(_pluginInterface);
                services.AddConfig(PluginConfig.Load(_pluginInterface));
                services.AddHaselCommon();
                services.AddLeveHelper();
            })
            .Build();
    }

    public Task LoadAsync(CancellationToken cancellationToken)
    {
        return _host.StartOnFrameworkThread(_framework, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _host.StopOnFrameworkThread(_framework).ConfigureAwait(false);
        }
        finally
        {
            _host.Dispose();
        }
    }
}
