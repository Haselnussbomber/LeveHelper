using System;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;

namespace LeveHelper;

public unsafe class DirectorHelper
{
    public Director* LastDirector = null;
    public Director* CurrentDirector => UIState.Instance()->ActiveDirector;

    public delegate void DirectorChangedEventHandler(Director* NewDirector);
    public event DirectorChangedEventHandler DirectorChanged = null!;

    public DirectorHelper()
    {
        SignatureHelper.Initialise(this);
    }

    public void Update()
    {
        if (LastDirector != CurrentDirector)
        {
            LastDirector = CurrentDirector;
            DirectorChanged?.Invoke(CurrentDirector);
        }
    }

    // oh my...
    [Signature("48 8D 05 ?? ?? ?? ?? 48 8B 91 ?? ?? ?? ?? 48 8B D9 48 89 01 48 85 D2 74 36 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 09 48 8B 01 FF 90 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 15 48 8B 01 BA ?? ?? ?? ?? FF 10 48 C7 83 ?? ?? ?? ?? ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 40 F6 C7 01 74 0D BA ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 48 8B C3 48 8B 5C 24 ?? 48 83 C4 20 5F C3 CC CC CC CC CC CC CC CC CC CC CC CC 48 89 5C 24 ?? 57 48 83 EC 20 48 8D 05 ?? ?? ?? ?? 8B DA 48 89 01 48 8B F9 E8 ?? ?? ?? ?? F6 C3 01 74 0D BA ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 48 8B C7 48 8B 5C 24 ?? 48 83 C4 20 5F C3 CC CC 40 53", ScanType = ScanType.StaticAddress)]
    private readonly IntPtr BattleLeveDirectorVtbl = IntPtr.Zero;

    [Signature("80 3D ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 48 8B 49 10", ScanType = ScanType.StaticAddress)] // somehow this resolves to the byte before
    private readonly byte* g_CurrentDirector_FullToDoUpdate = null;

    private IntPtr g_CurrentDirector_IsActive => (IntPtr)g_CurrentDirector_FullToDoUpdate + 1;

    [Signature("48 8B 01 8B 40 5C")]
    public readonly DirectorGetIconIdDelegate Director_GetIconId = null!;
    public delegate uint DirectorGetIconIdDelegate(Director* director);

    [Signature("E8 ?? ?? ?? ?? 85 C0 78 1E")]
    public readonly DirectorGetLevelDelegate Director_GetLevel = null!;
    public delegate int DirectorGetLevelDelegate(Director* director);

    [Signature("48 8B 09 48 8B 01 48 FF A0 A8 07 00 00")]
    public readonly DirectorHasTimerDelegate Director_HasTimer = null!;
    public delegate bool DirectorHasTimerDelegate(Director* director);

    [Signature("48 8B 09 48 8B 01 48 FF A0 A8 07 00 00")]
    public readonly DirectorHasEndedDelegate Director_HasEnded = null!;
    public delegate bool DirectorHasEndedDelegate(Director* director);

    [Signature("E8 ?? ?? ?? ?? 48 8B F0 B8 ?? ?? ?? ?? F7 E6")]
    public readonly DirectorGetSecondsLeftDelegate Director_GetSecondsLeft = null!;
    public delegate long DirectorGetSecondsLeftDelegate(Director* director);

    [Signature("40 53 48 83 EC 20 4C 8B 09 48 8D 59 70")]
    public readonly DirectorGetObjectiveDelegate Director_GetObjective = null!;
    public delegate Utf8String* DirectorGetObjectiveDelegate(Director* director);

    [Signature("48 8B 09 48 8B 01 48 FF A0 B0 07 00 00")]
    public readonly DirectorGetEventItemIdDelegate Director_GetEventItemId = null!;
    public delegate uint DirectorGetEventItemIdDelegate(Director* director);

    /* too much work right now
    [Signature("48 8B 09 48 8B 01 48 FF A0 C0 07 00 00")]
    public readonly DirectorGetStepsDelegate Director_GetSteps = null!;
    public delegate IntPtr DirectorGetStepsDelegate(Director* director);
    */

    /* no clue
    [Signature("48 8B 09 48 8B 01 48 FF A0 C8 07 00 00")]
    public readonly DirectorGetLuaEventHandlerVf258Delegate Director_GetLuaEventHandlerVf258 = null!;
    public delegate long DirectorGetLuaEventHandlerVf258Delegate(Director* director);
    */

    public bool IsBattleLeveDirector => CurrentDirector != null && *(IntPtr*)CurrentDirector == BattleLeveDirectorVtbl;
    public bool IsDirectorActive => CurrentDirector != null && *(byte*)g_CurrentDirector_IsActive == 0x01;
    public Utf8String* Title => (Utf8String*)(CurrentDirector + 8);
    public uint IconId => Director_GetIconId(CurrentDirector); // LeveAssignmentType.Icon
    public int Level => Director_GetLevel(CurrentDirector);
    public bool HasTimer => Director_HasTimer(CurrentDirector);
    public bool HasEnded => Director_HasEnded(CurrentDirector);
    public long SecondsLeft => Director_GetSecondsLeft(CurrentDirector);
    public Utf8String* Objective => Director_GetObjective(CurrentDirector); // from LeveString sheet
    public uint EventItemId => Director_GetEventItemId(CurrentDirector);
}
