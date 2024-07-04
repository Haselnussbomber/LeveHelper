using System.IO;
using Dalamud.Game;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Extensions;
using HaselCommon.Logger;
using InteropGenerator.Runtime;
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
            .Initialize(pluginInterface)

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
            .AddIServices<IFilter>()
            .AddSingleton<FiltersState>()
            .AddSingleton<FilterManager>()
            .AddSingleton<WindowState>()
            .AddSingleton<WantedTargetScanner>()
            .AddSingleton<LeveIssuerCache>()
            .AddSingleton<LeveRequiredItemsCache>()
#if DEBUG
            .AddSingleton<DebugTab>()
#endif
            .AddSingleton<ListTab>()
            .AddSingleton<QueueTab>()
            .AddSingleton<RecipeTreeTab>()

            // Windows
            .AddSingleton<MainWindow>()
            .AddSingleton<ConfigWindow>();

        Service.BuildProvider();

        // ---

        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        Resolver.GetInstance.Setup(
            sigScanner.SearchBase,
            dataManager.GameData.Repositories["ffxiv"].Version,
            new FileInfo(Path.Join(pluginInterface.ConfigDirectory.FullName, "SigCache.json")));
        Resolver.GetInstance.Resolve();

        // ---

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

/*
public sealed class Plugin : IDalamudPlugin
{
    private readonly CommandInfo CommandInfo;
    private bool IsOpenMainUiSubscribed = false;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.AddService(Configuration.Load());
        Service.AddService<WantedTargetScanner>();
        Service.AddService<FilterManager>();

        Service.ClientState.Login += SubscribeOpenMainUi;
        Service.ClientState.Logout += UnsubscribeOpenMainUi;
        Service.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        Service.PluginInterface.LanguageChanged += PluginInterface_LanguageChanged;

        CommandInfo = new CommandInfo(OnCommand) { HelpMessage = t("CommandHandlerHelpMessage") };

        Service.CommandManager.AddHandler("/levehelper", CommandInfo);
        Service.CommandManager.AddHandler("/lh", CommandInfo);

        if (Service.ClientState.IsLoggedIn)
            SubscribeOpenMainUi();
    }

    private void PluginInterface_LanguageChanged(string langCode)
    {
        CommandInfo.HelpMessage = t("CommandHandlerHelpMessage");

        Service.GetService<FilterManager>().Reload();
        Service.WindowManager.GetWindow<MainWindow>()?.OnLanguageChange();
    }

    private void OnCommand(string command, string arguments)
    {
        switch (arguments.ToLower())
        {
            case "config":
                Service.WindowManager.ToggleWindow<ConfigWindow>();
                break;

            default:
                Service.WindowManager.ToggleWindow<MainWindow>();
                break;
        }
    }

    private void SubscribeOpenMainUi()
    {
        if (!IsOpenMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
            IsOpenMainUiSubscribed = true;
        }
    }

    private void UnsubscribeOpenMainUi()
    {
        if (IsOpenMainUiSubscribed)
        {
            Service.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
            IsOpenMainUiSubscribed = false;
        }
    }

    private void ToggleMainWindow() => Service.WindowManager.ToggleWindow<MainWindow>();
    private void ToggleConfigWindow() => Service.WindowManager.ToggleWindow<ConfigWindow>();

    void IDisposable.Dispose()
    {
        Service.ClientState.Login -= SubscribeOpenMainUi;
        Service.ClientState.Logout -= UnsubscribeOpenMainUi;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;
        Service.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
        Service.PluginInterface.LanguageChanged -= PluginInterface_LanguageChanged;

        Service.CommandManager.RemoveHandler("/levehelper");
        Service.CommandManager.RemoveHandler("/lh");

        Service.Dispose();
    }
}
*/
