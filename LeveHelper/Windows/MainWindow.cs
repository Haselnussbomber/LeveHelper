using System.Numerics;
using AutoCtor;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using HaselCommon.Gui;
using HaselCommon.Services;
using LeveHelper.Tabs;

namespace LeveHelper.Windows;

[RegisterTransient, AutoConstruct]
public partial class MainWindow : SimpleWindow
{
    private readonly WindowManager _windowManager;
    private readonly TextService _textService;
    private readonly IClientState _clientState;

    private readonly QueueTab _queueTab;
    private readonly RecipeTreeTab _recipeTreeTab;
    private readonly ListTab _listTab;
#if DEBUG
    private readonly DebugTab _debugTab;
#endif

    [AutoPostConstruct]
    private void Initialize()
    {
        Size = new Vector2(940, 640);
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
                ImGui.TextUnformatted(_textService.Translate(_windowManager.TryGetWindow<ConfigWindow>(out var configWindow) && configWindow.IsOpen
                    ? "TitleBarButton.ToggleConfig.Tooltip.CloseConfig"
                    : "TitleBarButton.ToggleConfig.Tooltip.OpenConfig"));
            },
            Click = (button) => _windowManager.CreateOrToggle<ConfigWindow>()
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
        //_debugTab.Draw(this);
#endif
    }
}
