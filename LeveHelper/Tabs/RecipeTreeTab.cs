using System.Linq;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using LeveHelper.Records;
using LeveHelper.Utils;

namespace LeveHelper;

public class RecipeTreeTab
{
    private readonly WindowState _state;

    public RecipeTreeTab(WindowState state)
    {
        _state = state;
    }

    public void Draw()
    {
        using var windowId = ImRaii.PushId("##RecipeTreeTab");

        if (!QuestUtils.GetActiveLeveIds().Any())
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(t("RecipeTreeTab.NoActiveLevequests"));
            return;
        }

        ImGui.Spacing();
        _state.DrawIngredients("RecipeTree", _state.LeveRequiredItems, 1);
    }
}
