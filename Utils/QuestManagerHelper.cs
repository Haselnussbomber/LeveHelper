#pragma warning disable 0649
using System;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace LeveHelper;

public unsafe class QuestManagerHelper : IDisposable
{
    private static QuestManagerHelper instance = null!;
    public static QuestManagerHelper Instance => instance ??= new();

    private readonly QuestManager* questManager;

    private QuestManagerHelper()
    {
        SignatureHelper.Initialise(this);
        questManager = QuestManager.Instance();
    }

    // see: 88 05 ?? ?? ?? ?? 0F B7 41 06
    public byte NumAllowances => *(byte*)((IntPtr)questManager + 0xDB8);

    // see: 48 FF 0D ?? ?? ?? ?? 48 8D 4C 24
    public byte NumActiveLevequests => *(byte*)((IntPtr)questManager + 0xEB0);

    // see: E8 ?? ?? ?? ?? 41 F6 C6 04
    public bool IsLevequestCompleted(ushort rowId)
    {
        var pos = rowId >> 3;
        return pos < 211 && ((0x80 >> (rowId & 7)) & *(byte*)((IntPtr)questManager + 0xDC0 + pos)) != 0;
    }

    [Signature("E8 ?? ?? ?? ?? 41 8D 75 01")]
    private readonly delegate* unmanaged<long> _getNextAllowancesTimestamp;
    public DateTime NextAllowances => DateTimeOffset.FromUnixTimeSeconds(_getNextAllowancesTimestamp() * 60).LocalDateTime;

    void IDisposable.Dispose()
    {
        instance = null!;
    }
}
