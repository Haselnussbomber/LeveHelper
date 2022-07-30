using System.Numerics;

namespace LeveHelper;

public static class ImGuiUtils
{
    public static Vector4 ColorTransparent = new(0f, 0f, 0f, 0f);

    public static Vector4 ColorRed = new(1f, 0f, 0f, 1f);
    public static Vector4 ColorGreen = new(0f, 1f, 0f, 1f);

    public static Vector4 ColorMaelstorm = new(129f / 255f, 19f / 255f, 1f / 255f, 1f); // #811301
    public static Vector4 ColorAdder = new(165f / 255f, 115f / 255f, 0f, 1f); // #a57300
    public static Vector4 ColorLegion = new(72f / 255f, 89f / 255f, 55f / 255f, 1f); // #485937

    public static Vector4 ColorGroup = new(216f / 255f, 187f / 255f, 125f / 255f, 1f); // #D8BB7D
}
