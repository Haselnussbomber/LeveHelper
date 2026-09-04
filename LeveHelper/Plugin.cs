using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin;
using HaselCommon.Extensions;
using LeveHelper.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeveHelper;

[AutoConstruct]
public partial class Plugin : IAsyncDalamudPlugin
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private IHost _host;

    [AutoPostConstruct]
    private void Initialize()
    {
        _host = new HostBuilder()
            .UseContentRoot(_pluginInterface.AssemblyLocation.Directory!.FullName)
            .ConfigureHostOptions(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            })
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
        return _host.StartAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _host.StopAsync().ConfigureAwait(false);
        }
        finally
        {
            _host.Dispose();
        }
    }
}
