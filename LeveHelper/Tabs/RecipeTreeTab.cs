using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using HaselCommon.Services;
using LeveHelper.Services;

namespace LeveHelper.Tabs;

[RegisterSingleton]
public class RecipeTreeTab(CraftQueueState State, TextService TextService, LeveService LeveService)
{
    public void Draw(Window window)
    {
        using var tab = ImRaii.TabItem(TextService.Translate("Tabs.RecipeTree"));
        if (!tab) return;

        window.RespectCloseHotkey = true;

        using var windowId = ImRaii.PushId("##RecipeTreeTab");

        if (!LeveService.HasAcceptedLeveQuests())
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(TextService.Translate("RecipeTreeTab.NoActiveLevequests"));
            return;
        }

        ImGui.Spacing();
        State.DrawIngredients("RecipeTree", State.LeveRequiredItems, 1);
    }
}
