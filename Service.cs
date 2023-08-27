using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using LeveHelper.Services;

namespace LeveHelper;

public class Service
{
    public static AddonObserver AddonObserver { get; private set; } = null!;
    public static GameFunctions GameFunctions { get; private set; } = null!;
    public static StringManager StringManager { get; private set; } = null!;
    public static TextureManager TextureManager { get; private set; } = null!;
    public static TranslationManager TranslationManager { get; private set; } = null!;
    public static WantedTargetScanner WantedTargetScanner { get; private set; } = null!;
    public static WindowManager WindowManager { get; private set; } = null!;

    public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;

    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;

    public static void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
        PluginInterface.Create<Service>();
        AddonObserver = new();
        GameFunctions = new();
        StringManager = new();
        TextureManager = new();
        TranslationManager = new(PluginInterface, ClientState);
        WantedTargetScanner = new();
        WindowManager = new(pluginInterface);
    }

    public static void Dispose()
    {
        AddonObserver.Dispose();
        GameFunctions.Dispose();
        TextureManager.Dispose();
        TranslationManager.Dispose();
        WantedTargetScanner.Dispose();
        WindowManager.Dispose();
    }
}
