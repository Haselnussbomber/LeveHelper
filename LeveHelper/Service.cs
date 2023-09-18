using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon;
using HaselCommon.Services;
using LeveHelper.Services;

namespace LeveHelper;

public class Service
{
    public static AddonObserver AddonObserver { get; private set; } = null!;
    public static TranslationManager TranslationManager { get; private set; } = null!;
    public static TextureManager TextureManager { get; private set; } = null!;
    public static WindowManager WindowManager { get; private set; } = null!;

    public static GameFunctions GameFunctions { get; private set; } = null!;
    public static WantedTargetScanner WantedTargetScanner { get; private set; } = null!;

    public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;

    public static void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
        PluginInterface.Create<Service>();

        HaselCommonBase.Initialize(pluginInterface);
        AddonObserver = HaselCommonBase.AddonObserver;
        TranslationManager = HaselCommonBase.TranslationManager;
        TextureManager = HaselCommonBase.TextureManager;
        WindowManager = HaselCommonBase.WindowManager;

        GameFunctions = new();
        WantedTargetScanner = new();
    }

    public static void Dispose()
    {
        GameFunctions = null!;
        
        WantedTargetScanner.Dispose();
        WantedTargetScanner = null!;

        HaselCommonBase.Dispose();
        AddonObserver = null!;
        TranslationManager = null!;
        TextureManager = null!;
        WindowManager = null!;
    }
}
