using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Utility;
using ImGuiNET;
using ImGuiScene;
using Lumina.Data.Files;

namespace LeveHelper;

public static class ImGuiUtils
{
    public static Vector4 ColorTransparent = new(0f, 0f, 0f, 0f);

    public static Vector4 ColorRed = new(1f, 0f, 0f, 1f);
    public static Vector4 ColorYellow = new(1f, 1f, 0f, 1f);
    public static Vector4 ColorGreen = new(0f, 1f, 0f, 1f);

    public static Vector4 ColorMaelstorm = new(129f / 255f, 19f / 255f, 1f / 255f, 1f); // #811301
    public static Vector4 ColorAdder = new(165f / 255f, 115f / 255f, 0f, 1f); // #a57300
    public static Vector4 ColorLegion = new(72f / 255f, 89f / 255f, 55f / 255f, 1f); // #485937

    public static Vector4 ColorGroup = new(216f / 255f, 187f / 255f, 125f / 255f, 1f); // #D8BB7D

    private static readonly Dictionary<int, TextureWrap> icons = new();

    public static void DrawIcon(int iconId, int width = -1, int height = -1)
    {
        if (!icons.ContainsKey(iconId))
        {
            var tex = Service.Data.GetFile<TexFile>($"ui/icon/{iconId / 1000:D3}000/{iconId:D6}_hr1.tex");
            if (tex == null)
                return;

            var texWrap = Service.PluginInterface.UiBuilder.LoadImageRaw(tex.GetRgbaImageData(), tex.Header.Width, tex.Header.Height, 4);
            if (texWrap.ImGuiHandle == IntPtr.Zero)
                return;

            icons[iconId] = texWrap;
        }

        ImGui.Image(icons[iconId].ImGuiHandle, new(width == -1 ? icons[iconId].Width : width, height == -1 ? icons[iconId].Height : height));
    }
}
