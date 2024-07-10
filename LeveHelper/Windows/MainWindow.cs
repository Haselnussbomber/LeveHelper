using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using HaselCommon.Windowing;
using ImGuiNET;
using LeveHelper.Records;

namespace LeveHelper.Windows;

public class MainWindow : SimpleWindow
{
    private readonly TextService TextService;
    private readonly AddonObserver AddonObserver;
    private readonly IDalamudPluginInterface PluginInterface;
    private readonly IClientState ClientState;
    private readonly ICommandManager CommandManager;
    private readonly ConfigWindow ConfigWindow;
    private readonly LeveService LeveService;
    private readonly WindowState WindowState;
    private readonly FilterManager FilterManager;

    private readonly QueueTab QueueTab;
    private readonly RecipeTreeTab RecipeTreeTab;
    private readonly ListTab ListTab;
    private readonly DebugTab DebugTab;

    private readonly CommandInfo CommandInfo;

    private IEnumerable<ushort> LastActiveLevequestIds = [];

    public MainWindow(
        WindowManager windowManager,
        TextService textService,
        AddonObserver addonObserver,
        IDalamudPluginInterface pluginInterface,
        IClientState clientState,
        ICommandManager commandManager,
        ConfigWindow configWindow,
        LeveService leveService,
        WindowState windowState,
        FilterManager filterManager,
        DebugTab debugTab,
        QueueTab queueTab,
        RecipeTreeTab recipeTreeTab,
        ListTab listTab) : base(windowManager, textService.Translate("WindowTitle.Main"))
    {
        TextService = textService;
        AddonObserver = addonObserver;
        PluginInterface = pluginInterface;
        ClientState = clientState;
        CommandManager = commandManager;
        ConfigWindow = configWindow;
        LeveService = leveService;
        WindowState = windowState;
        FilterManager = filterManager;

        DebugTab = debugTab;
        QueueTab = queueTab;
        RecipeTreeTab = recipeTreeTab;
        ListTab = listTab;

        Size = new Vector2(830, 600);
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(4096, 2160)
        };

        TitleBarButtons.Add(new()
        {
            Icon = Dalamud.Interface.FontAwesomeIcon.Cog,
            IconOffset = new(0, 1),
            ShowTooltip = () => ImGui.SetTooltip(textService.Translate($"TitleBarButton.ToggleConfig.Tooltip.{(ConfigWindow.IsOpen ? "Close" : "Open")}Config")),
            Click = (button) => { ConfigWindow.Toggle(); }
        });

        ListTab = listTab;
        QueueTab = queueTab;
        RecipeTreeTab = recipeTreeTab;
#if DEBUG
        DebugTab = debugTab;
#endif

        CommandInfo = new CommandInfo((_, _) => Toggle())
        {
            HelpMessage = textService.Translate("LeveHelper.CommandHandlerHelpMessage")
        };

        CommandManager.AddHandler("/levehelper", CommandInfo);
        CommandManager.AddHandler("/lh", CommandInfo);

        PluginInterface.UiBuilder.OpenMainUi += Toggle;
        PluginInterface.UiBuilder.OpenConfigUi += ConfigWindow.Toggle;
        TextService.LanguageChanged += OnLanguageChanged;
        AddonObserver.AddonOpen += OnAddonOpen;
        AddonObserver.AddonClose += OnAddonClose;
    }

    public new void Dispose()
    {
        AddonObserver.AddonOpen -= OnAddonOpen;
        AddonObserver.AddonClose -= OnAddonClose;
        TextService.LanguageChanged -= OnLanguageChanged;
        PluginInterface.UiBuilder.OpenConfigUi -= ConfigWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi -= Toggle;
        CommandManager.RemoveHandler("/lh");
        CommandManager.RemoveHandler("/levehelper");
        base.Dispose();
    }

    private void OnLanguageChanged(string langCode)
    {
        CommandInfo.HelpMessage = TextService.Translate("LeveHelper.CommandHandlerHelpMessage");
    }

    private void Refresh()
    {
        WindowState.UpdateList();
        FilterManager.Update();
    }

    public void OnLanguageChange() => Refresh();

    private void OnAddonOpen(string addonName)
    {
        if (addonName is "Catch")
            Refresh();
    }

    private void OnAddonClose(string addonName)
    {
        if (addonName is "Synthesis" or "SynthesisSimple" or "Gathering" or "ItemSearchResult" or "InclusionShop" or "Shop" or "ShopExchangeCurrency" or "ShopExchangeItem")
            Refresh();
    }

    public override void Update()
    {
        var activeLevequestIds = LeveService.GetActiveLeveIds();
        if (!LastActiveLevequestIds.SequenceEqual(activeLevequestIds))
        {
            LastActiveLevequestIds = activeLevequestIds;
            Refresh();
        }
    }

    public override bool DrawConditions() => ClientState.IsLoggedIn;

    public override void Draw()
    {
        using var tabbar = ImRaii.TabBar("LeveHelperTabs", ImGuiTabBarFlags.Reorderable);
        if (!tabbar) return;

        ListTab.Draw(this);
        QueueTab.Draw(this);
        RecipeTreeTab.Draw(this);
        DebugTab.Draw(this);
    }
}
