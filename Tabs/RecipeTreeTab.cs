using Dalamud.Interface;
using Dalamud.Interface.Raii;
using ImGuiNET;
using LeveHelper.Utils;

namespace LeveHelper;

public class RecipeTreeTab
{
    public PluginWindow Window { get; }

    public RecipeTreeTab(PluginWindow window)
    {
        Window = window;
    }

    public void Draw()
    {
        using var windowId = ImRaii.PushId("##RecipeTreeTab");

        if (Service.GameFunctions.ActiveLevequestsIds.Length == 0)
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y / 2f - ImGui.GetFrameHeight() / 2f);
            ImGuiHelpers.CenteredText("No active Levequests");
            return;
        }

        if (ImGuiUtils.IconButton("##Refresh", FontAwesomeIcon.RedoAlt, "Refresh"))
        {
            Window.UpdateList();
        }

        Window.DrawIngredients("RecipeTree", Window.LeveRequiredItems, 1);
    }
}
