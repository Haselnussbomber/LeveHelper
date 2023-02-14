using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using static LeveHelper.ImGuiUtils;

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
        if (Service.GameFunctions.ActiveLevequestsIds.Length == 0)
        {
            ImGui.TextDisabled("No active Levequests"); // center?
            return;
        }

        if (ImGuiComponents.IconButton(FontAwesomeIcon.RedoAlt))
        {
            Window.UpdateList();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.SetTooltip("Refresh");
        }

        DrawIngredients("RecipeTree", Window.LeveRequiredItems, 1);
    }
}
