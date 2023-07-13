using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace LeveHelper.Utils;

public static partial class ImGuiUtils
{
    public static void DrawFontAwesomeIcon(FontAwesomeIcon icon, Vector4 color)
    {
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            ImGui.TextColored(color, icon.ToIconString());
        }
        ImGui.SameLine();
    }

    public static bool IsInViewport()
    {
        var distanceY = ImGui.GetCursorPosY() - ImGui.GetScrollY();
        return distanceY >= 0 && distanceY <= ImGui.GetWindowHeight();
    }
}
