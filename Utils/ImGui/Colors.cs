using System.Numerics;
using LeveHelper.Structs;

namespace LeveHelper.Utils;

public static class Colors
{
    public static ImColor Transparent { get; } = Vector4.Zero;
    public static ImColor White { get; } = Vector4.One;
    public static ImColor Black { get; } = new(0, 0, 0);
    public static ImColor Orange { get; } = new(1, 0.6f, 0);
    public static ImColor Gold { get; } = new(0.847f, 0.733f, 0.49f);
    public static ImColor Red { get; } = new(1, 0, 0);
    public static Vector4 Freesia { get; } = new(246 / 255f, 195 / 255f, 36 / 255f, 1f); // #F6C324
    public static Vector4 Yellow { get; } = new(1f, 1f, 0f, 1f);
    public static Vector4 YellowGreen { get; } = new(154 / 255f, 205 / 255f, 50 / 255f, 1f); // #9ACD32
    public static ImColor Green { get; } = new(0, 1, 0);
    public static ImColor Grey { get; } = new(0.73f, 0.73f, 0.73f);
    public static ImColor Grey2 { get; } = new(0.87f, 0.87f, 0.87f);
    public static ImColor Grey3 { get; } = new(0.6f, 0.6f, 0.6f);
    public static ImColor Grey4 { get; } = new(0.3f, 0.3f, 0.3f);

    public static Vector4 Maelstorm { get; } = new(129f / 255f, 19f / 255f, 1f / 255f, 1f); // #811301
    public static Vector4 Adder { get; } = new(165f / 255f, 115f / 255f, 0f, 1f); // #a57300
    public static Vector4 Legion { get; } = new(72f / 255f, 89f / 255f, 55f / 255f, 1f); // #485937

    public static Vector4 Group { get; } = new(216f / 255f, 187f / 255f, 125f / 255f, 1f); // #D8BB7D
}
