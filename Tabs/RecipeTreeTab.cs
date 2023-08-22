using Dalamud.Interface;
using Dalamud.Interface.Raii;
using ImGuiNET;
using LeveHelper.Windows;

namespace LeveHelper;

public class RecipeTreeTab
{
    public MainWindow Window { get; }

    public RecipeTreeTab(MainWindow window)
    {
        Window = window;
    }

    public void Draw()
    {
        using var windowId = ImRaii.PushId("##RecipeTreeTab");

        if (Service.GameFunctions.ActiveLevequestsIds.Length == 0)
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText(t("RecipeTreeTab.NoActiveLevequests"));
            return;
        }

        Window.DrawIngredients("RecipeTree", Window.LeveRequiredItems, 1);
    }
}
