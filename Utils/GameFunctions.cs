#pragma warning disable 0649
using System;
using System.Collections.Generic;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace LeveHelper;

public unsafe class GameFunctions
{
    public GameFunctions()
    {
        SignatureHelper.Initialise(this);
    }

    [Signature("E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 88 45 80")]
    public readonly IsLevequestCompletedDelegate IsLevequestCompleted = null!;
    public delegate bool IsLevequestCompletedDelegate(QuestManager* questManager, ushort id);

    [Signature("E8 ?? ?? ?? ?? 41 8D 75 01")]
    private readonly delegate* unmanaged<long> _getNextAllowancesTimestamp;
    public DateTime NextAllowances => DateTimeOffset.FromUnixTimeSeconds(_getNextAllowancesTimestamp() * 60).LocalDateTime;

    [Signature("88 05 ?? ?? ?? ?? 0F B7 41 06", ScanType = ScanType.StaticAddress)]
    public readonly byte* NumAllowancesPtr = null!;
    public byte NumAllowances => *NumAllowancesPtr;

    [Signature("48 FF 0D ?? ?? ?? ?? 48 8D 4C 24", ScanType = ScanType.StaticAddress)]
    public readonly byte* NumActiveLevequestsPtr = null!;
    public byte NumActiveLevequests => *NumActiveLevequestsPtr;

    [Signature("E9 ?? ?? ?? ?? 48 8D 47 30")]
    private readonly FormatObjectStringDelegate FormatObjectString = null!; // how do you expect me to name things i have no clue about
    private delegate IntPtr FormatObjectStringDelegate(int mode, uint id, uint idConversionMode, uint a4);

    private readonly Dictionary<uint, string> ENpcResidentNameCache = new();
    public string GetENpcResidentName(uint npcId)
    {
        if (!ENpcResidentNameCache.TryGetValue(npcId, out var name))
        {
            name = MemoryHelper.ReadSeStringNullTerminated(FormatObjectString(0, npcId, 3, 1)).ToString();
            ENpcResidentNameCache.Add(npcId, name);
        }

        return name;
    }
}
