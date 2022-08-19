namespace LeveHelper;

public static class LuminaSeStringExtension
{
    public static string ClearString(this Lumina.Text.SeString str)
    {
        return Dalamud.Game.Text.SeStringHandling.SeString.Parse(str.RawData).ToString();
    }
}
