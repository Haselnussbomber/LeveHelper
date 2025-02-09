using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using HaselCommon.Gui;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Tabs;

namespace LeveHelper.Windows;

[RegisterSingleton]
public class MainWindow : SimpleWindow
{
    private readonly IClientState _clientState;

    private readonly QueueTab _queueTab;
    private readonly RecipeTreeTab _recipeTreeTab;
    private readonly ListTab _listTab;
#if DEBUG
    private readonly DebugTab _debugTab;
#endif

    public MainWindow(
        WindowManager windowManager,
        TextService textService,
        IClientState clientState,
        ConfigWindow configWindow,

        // Tabs
#if DEBUG
        DebugTab debugTab,
#endif
        QueueTab queueTab,
        RecipeTreeTab recipeTreeTab,
        ListTab listTab) : base(windowManager, textService.Translate("WindowTitle.Main"))
    {
        _clientState = clientState;

        _listTab = listTab;
        _queueTab = queueTab;
        _recipeTreeTab = recipeTreeTab;
#if DEBUG
        _debugTab = debugTab;
#endif

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
            ShowTooltip = () =>
            {
                using var tooltip = ImRaii.Tooltip();
                textService.Draw(windowManager.TryGetWindow<ConfigWindow>(out var configWindow) && configWindow.IsOpen
                    ? "TitleBarButton.ToggleConfig.Tooltip.CloseConfig"
                    : "TitleBarButton.ToggleConfig.Tooltip.OpenConfig");
            },
            Click = (button) =>
            {
                if (windowManager.TryGetWindow<ConfigWindow>(out var configWindow))
                    configWindow.Toggle();
                else
                    windowManager.CreateOrOpen(Service.Get<ConfigWindow>);
            }
        });
    }

    public override bool DrawConditions()
    {
        return _clientState.IsLoggedIn;
    }

    public override void Draw()
    {
        using var tabbar = ImRaii.TabBar("LeveHelperTabs", ImGuiTabBarFlags.Reorderable);
        if (!tabbar) return;

        _listTab.Draw(this);
        _queueTab.Draw(this);
        _recipeTreeTab.Draw(this);
#if DEBUG
        _debugTab.Draw(this);
#endif
    }
}
