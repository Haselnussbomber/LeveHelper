using System.Linq;
using System.Numerics;
using Dalamud.Interface.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using LeveHelper.Records;

namespace LeveHelper.Windows;

public unsafe class MainWindow : Window, IDisposable
{
    private readonly WindowState _state;

    private readonly QueueTab _queueTab;
    private readonly RecipeTreeTab _recipeTreeTab;
    private readonly ListTab _listTab;
#if DEBUG
    private readonly DebugTab _debugTab;
#endif

    private ushort[] _lastActiveLevequestIds = Array.Empty<ushort>();

    public MainWindow() : base(t("WindowTitle.Main"))
    {
        Namespace = "LeveHelper";

        Size = new Vector2(830, 600);
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(4096, 2160)
        };

        _state = new();

        _listTab = new(_state);
        _queueTab = new(_state);
        _recipeTreeTab = new(_state);
#if DEBUG
        _debugTab = new(_state);
#endif

        IsOpen = true;

        Service.AddonObserver.AddonOpen += OnAddonOpen;
        Service.AddonObserver.AddonClose += OnAddonClose;
    }

    public void Dispose()
    {
        Service.AddonObserver.AddonOpen -= OnAddonOpen;
        Service.AddonObserver.AddonClose -= OnAddonClose;
    }

    private void Refresh()
    {
        _state.UpdateList();
        Plugin.FilterManager.Update();
    }

    public void OnLanguageChange() => Refresh();

    public override void OnClose()
    {
        Service.WindowManager.CloseWindow<MainWindow>();
    }

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
        var activeLevequestIds = Service.GameFunctions.ActiveLevequestsIds;
        if (!_lastActiveLevequestIds.SequenceEqual(activeLevequestIds))
        {
            _lastActiveLevequestIds = activeLevequestIds;
            Refresh();
        }
    }

    public override bool DrawConditions() => Service.ClientState.IsLoggedIn;

    public override void Draw()
    {
        using (ImRaii.TabBar("LeveHelperTabs", ImGuiTabBarFlags.Reorderable))
        {
            using (var tab = ImRaii.TabItem(t("Tabs.Levequest")))
            {
                if (tab.Success)
                {
                    RespectCloseHotkey = true;

                    _listTab.Draw();
                }
            }

            using (var tab = ImRaii.TabItem(t("Tabs.Queue")))
            {
                if (tab.Success)
                {
                    RespectCloseHotkey = false;

                    _queueTab.Draw();
                }
            }

            using (var tab = ImRaii.TabItem(t("Tabs.RecipeTree")))
            {
                if (tab.Success)
                {
                    RespectCloseHotkey = true;

                    _recipeTreeTab.Draw();
                }
            }

#if false
            using (var tab = ImRaii.TabItem("Debug"))
            {
                if (tab.Success)
                {
                    RespectCloseHotkey = true;

                    _debugTab.Draw();
                }
            }
#endif
        }
    }
}
