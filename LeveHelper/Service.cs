using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon;
using HaselCommon.Services;
using LeveHelper.Services;

namespace LeveHelper;

public class Service
{
    public static AddonObserver AddonObserver => HaselCommonBase.AddonObserver;
    public static TranslationManager TranslationManager => HaselCommonBase.TranslationManager;
    public static TextureManager TextureManager => HaselCommonBase.TextureManager;
    public static WindowManager WindowManager => HaselCommonBase.WindowManager;

    public static GameFunctions GameFunctions { get; private set; } = null!;
    public static WantedTargetScanner WantedTargetScanner { get; private set; } = null!;

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
        GameFunctions = new();
        WantedTargetScanner = new();
    }

    public static void Dispose()
    {
        GameFunctions.Dispose();
        WantedTargetScanner.Dispose();
    }
}
