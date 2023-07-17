using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Memory;
using Dalamud.Plugin;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace LeveHelper;

public unsafe class Plugin : IDalamudPlugin, IDisposable
{
    public string Name => "LeveHelper";

    internal static WindowSystem WindowSystem = new("LeveHelper");
    internal static PluginWindow? PluginWindow;

    internal static Configuration Config = null!;
    internal static FilterManager FilterManager = null!;
    internal static byte StartTown;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Service.TextureCache = new();
        Task.Run(Setup);
    }

    private void Setup()
    {
        Service.GameFunctions = new();
        SignatureHelper.Initialise(this);
        AddonSetupHook?.Enable();
        AddonFinalizeHook?.Enable();

        Config = Configuration.Load();
        PlaceNameHelper.Connect();
        WantedTargetScanner.Connect();

        FilterManager = new();

        Service.PluginInterface.UiBuilder.Draw += OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += ToggleWindow;

        var commandInfo = new CommandInfo(OnCommand)
        {
            HelpMessage = "Show Window"
        };

        Service.Commands.AddHandler("/levehelper", commandInfo);
        Service.Commands.AddHandler("/lh", commandInfo);

        GatheringPointCache.Load();
    }

    private void OnDraw()
    {
        try
        {
            WindowSystem.Draw();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unexpected exception in OnDraw");
        }
    }

    private void OnCommand(string command, string args)
    {
        ToggleWindow();
    }

    private static void OpenWindow()
    {
        WindowSystem.AddWindow(PluginWindow = new PluginWindow());
    }

    internal static void CloseWindow()
    {
        if (PluginWindow == null)
            return;

        WindowSystem.RemoveWindow(PluginWindow);
        PluginWindow = null;
    }

    private static void ToggleWindow()
    {
        if (PluginWindow == null)
            OpenWindow();
        else
            CloseWindow();
    }

    void IDisposable.Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= ToggleWindow;

        PlaceNameHelper.Disconnect();
        WantedTargetScanner.Disconnect();

        Service.Commands.RemoveHandler("/levehelper");
        Service.Commands.RemoveHandler("/lh");

        WindowSystem.RemoveAllWindows();
        WindowSystem = null!;
        PluginWindow = null;

        Config.Save();
        Config = null!;

        FilterManager = null!;

        Service.TextureCache.Dispose();
    }

    [Signature("E8 ?? ?? ?? ?? 8B 83 ?? ?? ?? ?? C1 E8 14 ?? ??", DetourName = nameof(AddonSetup))]
    private Hook<AddonSetupDelegate> AddonSetupHook { get; init; } = null!;
    private delegate void AddonSetupDelegate(AtkUnitBase* unitBase);

    public void AddonSetup(AtkUnitBase* unitBase)
    {
        AddonSetupHook.Original(unitBase);

        if (unitBase != null)
            PluginWindow?.OnAddonOpen(MemoryHelper.ReadString((nint)unitBase->Name, 0x20), unitBase);
    }


    [Signature("E8 ?? ?? ?? ?? 48 8B 7C 24 ?? 41 8B C6 ?? ?? ??", DetourName = nameof(AddonFinalize))]
    private Hook<AddonFinalizeDelegate> AddonFinalizeHook { get; init; } = null!;
    private delegate void AddonFinalizeDelegate(AtkUnitManager* unitManager, AtkUnitBase** unitBase);
    public void AddonFinalize(AtkUnitManager* unitManager, AtkUnitBase** unitBasePtr)
    {
        var unitBase = *unitBasePtr;
        if (unitBase != null)
            PluginWindow?.OnAddonClose(MemoryHelper.ReadString((nint)unitBase->Name, 0x20), unitBase);

        AddonFinalizeHook.Original(unitManager, unitBasePtr);
    }
}
