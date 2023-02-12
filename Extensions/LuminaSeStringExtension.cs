using Dalamud.Utility;

namespace LeveHelper;

public static class LuminaSeStringExtension
{
    public static string ClearString(this Lumina.Text.SeString str)
    {
        return str.ToDalamudString().ToString();
    }
}
