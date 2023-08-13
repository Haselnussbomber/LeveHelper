using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using LeveHelper.Services;

namespace LeveHelper;

public class Service
{
    public static DalamudPluginInterface PluginInterface { get; internal set; } = null!;

    public static AddonObserver AddonObserver { get; internal set; } = null!;
    public static GameFunctions GameFunctions { get; internal set; } = null!;
    public static StringManager StringManager { get; internal set; } = null!;
    public static TextureManager TextureManager { get; internal set; } = null!;
    public static TranslationManager TranslationManager { get; internal set; } = null!;
    public static WantedTargetScanner WantedTargetScanner { get; internal set; } = null!;

    [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;

    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;

    public static void Initialize()
    {
        PluginInterface.Create<Service>();
        AddonObserver = new();
        GameFunctions = new();
        StringManager = new();
        TextureManager = new();
        TranslationManager = new();
        WantedTargetScanner = new();
    }

    public static void Dispose()
    {
        AddonObserver.Dispose();
        GameFunctions.Dispose();
        StringManager.Dispose();
        TextureManager.Dispose();
        TranslationManager.Dispose();
        WantedTargetScanner.Dispose();
    }
}
