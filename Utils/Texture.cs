using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using ImGuiScene;

namespace LeveHelper.Utils;

public class Texture : IDisposable
{
    public Texture(string path, int version)
    {
        Path = path;
        Version = version;
    }

    public string Path { get; }
    public int Version { get; }

    public TextureWrap? TextureWrap { get; private set; }

    public void Dispose()
    {
        TextureWrap?.Dispose();
        TextureWrap = null;
    }

    public void Draw(Vector2? drawSize = null)
    {
        var distanceY = ImGui.GetCursorPosY() - ImGui.GetScrollY();
        if (distanceY < 0 || distanceY > ImGui.GetWindowHeight())
        {
            ImGui.Dummy(drawSize ?? default);
            return;
        }

#if DEBUG
        if (TextureWrap == null)
        {
            PluginLog.Verbose($"[Texture] Loading {Path}");
        }
#endif

        TextureWrap ??= Service.Data.GetImGuiTexture(Path);

        if (TextureWrap == null || TextureWrap.ImGuiHandle == 0)
        {
            ImGui.Dummy(drawSize ?? default);
            return;
        }

        ImGui.Image(TextureWrap.ImGuiHandle, drawSize ?? new(TextureWrap.Width, TextureWrap.Height));
    }

    public void DrawPart(Vector2 partStart, Vector2 partSize, Vector2? drawSize = null)
    {
        TextureWrap ??= Service.Data.GetImGuiTexture(Path);

        if (TextureWrap == null || TextureWrap.ImGuiHandle == 0)
        {
            ImGui.Dummy(drawSize ?? default);
            return;
        }

        var texSize = new Vector2(TextureWrap.Width, TextureWrap.Height);

        partStart *= Version;
        partSize *= Version;

        var partEnd = (partStart + partSize) / texSize;
        partStart /= texSize;

        ImGui.Image(TextureWrap.ImGuiHandle, drawSize ?? partSize, partStart, partEnd);
    }
}
