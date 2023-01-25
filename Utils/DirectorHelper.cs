using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace LeveHelper;

public static unsafe class DirectorHelper
{
    private static Director* LastDirector = null;
    public static Director* ActiveDirector => UIState.Instance()->ActiveDirector;

    public delegate void DirectorChangedEventHandler(Director* NewDirector);
    public static event DirectorChangedEventHandler DirectorChanged = null!;

    public static void Update()
    {
        if (LastDirector != ActiveDirector)
        {
            LastDirector = ActiveDirector;
            DirectorChanged?.Invoke(ActiveDirector);
        }
    }

    public static bool IsBattleLeveDirectorActive =>
        ActiveDirector != null &&
        ActiveDirector->EventHandlerInfo != null &&
        ActiveDirector->EventHandlerInfo->EventId.Type == EventHandlerType.BattleLeveDirector;
}
