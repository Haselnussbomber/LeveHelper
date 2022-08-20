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

    // see: 88 05 ?? ?? ?? ?? 0F B7 41 06
    public byte NumAllowances => *(byte*)((IntPtr)QuestManager.Instance() + 0xDB8);

    // see: 48 FF 0D ?? ?? ?? ?? 48 8D 4C 24
    public byte NumActiveLevequests => *(byte*)((IntPtr)QuestManager.Instance() + 0xEB0);

    [Signature("E9 ?? ?? ?? ?? 48 8D 47 30")]
    private readonly FormatObjectStringDelegate FormatObjectString = null!; // how do you expect me to name things i have no clue about
    private delegate IntPtr FormatObjectStringDelegate(int mode, uint id, uint idConversionMode, uint a4);

    private readonly Dictionary<uint, string> ENpcResidentNameCache = new();
    public string GetENpcResidentName(uint npcId)
    {
        if (ENpcResidentNameCache.ContainsKey(npcId))
        {
            return ENpcResidentNameCache[npcId];
        }

        var ret = MemoryHelper.ReadSeStringNullTerminated(FormatObjectString(0, npcId, 3, 1)).ToString();

        ENpcResidentNameCache.Add(npcId, ret.ToString());

        return ret;
    }
}
