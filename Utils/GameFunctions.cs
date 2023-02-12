#pragma warning disable 0649
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public unsafe class GameFunctions
{
    public GameFunctions()
    {
        SignatureHelper.Initialise(this);
    }

    // TODO: remove when client structs is updated
    [Signature("E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 88 45 80")]
    public readonly IsLevequestCompletedDelegate IsLevequestCompleted = null!;
    public delegate bool IsLevequestCompletedDelegate(QuestManager* questManager, ushort id);

    // TODO: remove when client structs is updated
    [Signature("E8 ?? ?? ?? ?? 41 8D 75 01")]
    private readonly delegate* unmanaged<long> _getNextAllowancesTimestamp;
    public DateTime NextAllowances => DateTimeOffset.FromUnixTimeSeconds(_getNextAllowancesTimestamp() * 60).LocalDateTime;

    // TODO: remove when client structs is updated
    [Signature("88 05 ?? ?? ?? ?? 0F B7 41 06", ScanType = ScanType.StaticAddress)]
    public readonly byte* NumAllowancesPtr = null!;
    public byte NumAllowances => *NumAllowancesPtr;

    private unsafe Span<LeveWork> LeveQuestsSpan
    {
        get
        {
            var ptr = (LeveWork*)((nint)QuestManager.Instance() + 0xC80); // TODO: remove when client structs is updated (offset Patch 6.31)
            return new Span<LeveWork>(Unsafe.AsPointer(ref ptr[0]), 16);
            //return QuestManager.Instance()->LeveQuestsSpan;
        }
    }

    public int NumActiveLevequests
    {
        get
        {
            var numActiveLevequests = 0;

            var span = LeveQuestsSpan;
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].LeveId != 0)
                    numActiveLevequests++;
            }

            return numActiveLevequests;
        }
    }

    public ushort[] ActiveLevequestsIds
    {
        get
        {
            var ids = new List<ushort>();

            var span = LeveQuestsSpan;
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].LeveId != 0)
                    ids.Add(span[i].LeveId);
            }

            return ids.ToArray();
        }
    }

    public CachedLeve[] ActiveLevequests
    {
        get
        {
            var ids = new List<CachedLeve>();

            var span = LeveQuestsSpan;
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].LeveId != 0)
                    ids.Add(LeveCache.Get(span[i].LeveId));
            }

            return ids.ToArray();
        }
    }

    public bool IsLevequestAccepted(uint leveId)
    {
        var span = LeveQuestsSpan;
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i].LeveId == leveId)
                return true;
        }

        return false;
    }

    [Signature("E8 ?? ?? ?? ?? EB 1D 83 F8 0D")]
    private readonly ItemFinderModuleSearchForItemDelegate ItemFinderModule_SearchForItem = null!;
    private delegate void* ItemFinderModuleSearchForItemDelegate(void* module, uint itemId, bool isHQ = false);
    public void SearchForItem(uint itemId) => ItemFinderModule_SearchForItem(Framework.Instance()->GetUiModule()->GetItemFinderModule(), itemId);

    [Signature("E9 ?? ?? ?? ?? 48 8D 47 30")]
    private readonly FormatObjectStringDelegate FormatObjectString = null!;
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

    public static bool OpenMapWithMapLink(Level level) => Service.GameGui.OpenMapWithMapLink(GetMapLink(level));
    public static bool OpenMapWithMapLink(TerritoryType territoryType, float x, float y) =>
        Service.GameGui.OpenMapWithMapLink(new MapLinkPayload(territoryType.RowId, territoryType.Map.Row, x, y, 0f));

    public bool OpenMapWithGatheringPoint(FishingSpot? fishingSpot)
    {
        if (fishingSpot == null)
        {
            return false;
        }

        static int convert(short pos, ushort scale) => (pos - 1024) / (scale / 100);

        var territoryType = fishingSpot.TerritoryType.Value;
        var scale = territoryType!.Map.Value!.SizeFactor;
        var x = convert(fishingSpot.X, scale);
        var y = convert(fishingSpot.Z, scale);
        var radius = fishingSpot.Radius / 7 / (scale / 100);
        var iconId = 60465u;

        var agentMap = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
        agentMap->AgentInterface.Hide();
        agentMap->OpenMap(territoryType.Map.Row, territoryType.RowId, "", FFXIVClientStructs.FFXIV.Client.UI.Agent.MapType.GatheringLog);
        agentMap->AddGatheringTempMarker(x, y, radius, iconId, 4u, null);

        return true;
    }

    private static MapLinkPayload GetMapLink(Level level)
    {
        /// <see cref="https://github.com/xivapi/ffxiv-datamining/blob/master/docs/MapCoordinates.md"/>
        static float toCoord(float value, ushort scale)
        {
            var tileScale = 2048f / 41f;
            return value / tileScale + 2048f / (scale / 100f) / tileScale / 2 + 1;
        }

        return new MapLinkPayload(
            level.Territory.Row,
            level.Map.Row,
            toCoord(level.X, level.Map.Value?.SizeFactor ?? 100) + level.Map.Value?.OffsetX ?? 0,
            toCoord(level.Z, level.Map.Value?.SizeFactor ?? 100) + level.Map.Value?.OffsetY ?? 0,
            0.05f
        );
    }
}
