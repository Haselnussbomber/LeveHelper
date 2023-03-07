#pragma warning disable 0649
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public unsafe class GameFunctions
{
    public GameFunctions()
    {
        SignatureHelper.Initialise(this);
    }

    public ushort[] ActiveLevequestsIds
    {
        get
        {
            var ids = new List<ushort>();

            var span = QuestManager.Instance()->LeveQuestsSpan;
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

            var span = QuestManager.Instance()->LeveQuestsSpan;
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].LeveId != 0)
                    ids.Add(LeveCache.Get(span[i].LeveId));
            }

            return ids.ToArray();
        }
    }

    [Signature("66 89 54 24 ?? 66 89 4C 24 ?? 53")]
    public readonly CalculateTeleportCostDelegate CalculateTeleportCost = null!;
    public delegate uint CalculateTeleportCostDelegate(uint fromTerritoryTypeId, uint toTerritoryTypeId, bool a3, bool a4, bool a5);

    [Signature("80 F9 07 77 10")]
    public readonly IsGatheringPointTypeOffDelegate IsGatheringPointRare = null!;
    public delegate bool IsGatheringPointTypeOffDelegate(byte gatheringPointType);

    [Signature("E8 ?? ?? ?? ?? 41 B0 07")]
    public readonly FormatAddonTextDelegate FormatAddonText = null!;
    public delegate byte* FormatAddonTextDelegate(RaptureTextModule* module, uint id, int value);

    [Signature("E8 ?? ?? ?? ?? 4C 8B 05 ?? ?? ?? ?? 48 8D 8C 24 ?? ?? ?? ?? 48 8B D0 E8 ?? ?? ?? ?? 8B 4E 08")]
    public readonly GetGatheringPointNameDelegate GetGatheringPointName = null!;
    public delegate byte* GetGatheringPointNameDelegate(RaptureTextModule** module, byte gatheringType, byte gatheringPointType);

    [Signature("E8 ?? ?? ?? ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 30 5B C3 48 8B CB")]
    public readonly AgentJournal_OpenForQuestDelegate AgentJournal_OpenForQuest = null!;
    public delegate byte* AgentJournal_OpenForQuestDelegate(nint agentJournal, int id, int type, ushort a4 = 0, bool a5 = true); // type: 1 = Quest, 2 = Levequest

    private readonly Dictionary<uint, string> ENpcResidentNameCache = new();
    public string GetENpcResidentName(uint npcId)
    {
        if (!ENpcResidentNameCache.TryGetValue(npcId, out var name))
        {
            var textPtr = RaptureTextModule.Instance()->FormatAddonText2(2025, (int)npcId, 1);
            name = MemoryHelper.ReadSeStringNullTerminated((nint)textPtr).ToString();
            ENpcResidentNameCache.Add(npcId, name);
        }

        return name;
    }

    public bool OpenMapWithGatheringPoint(GatheringPoint? gatheringPoint, CachedItem? item = null)
    {
        if (gatheringPoint == null)
            return false;

        var territoryType = gatheringPoint.TerritoryType.Value;
        if (territoryType == null)
            return false;

        var gatheringPointBase = gatheringPoint.GatheringPointBase.Value;
        if (gatheringPointBase == null)
            return false;

        var exportedPoint = Service.Data.GetExcelSheet<ExportedGatheringPoint>()?.GetRow(gatheringPointBase.RowId);
        if (exportedPoint == null)
            return false;

        var gatheringType = exportedPoint.GatheringType.Value;
        if (gatheringType == null)
            return false;

        var raptureTextModule = Framework.Instance()->GetUiModule()->GetRaptureTextModule();

        var levelText = gatheringPointBase.GatheringLevel == 1
            ? raptureTextModule->GetAddonText(242) // "Lv. ???"
            : FormatAddonText(raptureTextModule, 35, gatheringPointBase.GatheringLevel);
        var space = MemoryUtils.FromString(" ");
        var gatheringPointName = GetGatheringPointName(
            &raptureTextModule,
            (byte)exportedPoint.GatheringType.Row,
            exportedPoint.GatheringPointType
        );

        var tooltipPtr = MemoryUtils.strconcat(levelText, space, gatheringPointName);
        var tooltip = IMemorySpace.GetDefaultSpace()->Create<Utf8String>();
        tooltip->SetString(tooltipPtr);

        var iconId = !IsGatheringPointRare(exportedPoint.GatheringPointType)
            ? gatheringType.IconMain
            : gatheringType.IconOff;

        var agentMap = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
        agentMap->TempMapMarkerCount = 0;
        agentMap->AddGatheringTempMarker(
            4u,
            (int)Math.Round(exportedPoint.X),
            (int)Math.Round(exportedPoint.Y),
            (uint)iconId,
            exportedPoint.Radius,
            tooltip
        );

        var titleBuilder = new SeStringBuilder()
            .Add(new TextPayload("LeveHelper"));
        if (item != null)
        {
            titleBuilder
                .Add(new TextPayload(" ("))
                .AddUiForeground(549)
                .AddUiGlow(550)
                .Add(new TextPayload(item.Name))
                .AddUiGlowOff()
                .AddUiForegroundOff()
                .Add(new TextPayload(")"));
        }
        var titlePtr = MemoryUtils.FromByteArray(titleBuilder.BuiltString.Encode());
        var title = IMemorySpace.GetDefaultSpace()->Create<Utf8String>();
        title->SetString(titlePtr);

        var mapInfo = stackalloc OpenMapInfo[1];
        mapInfo->Type = FFXIVClientStructs.FFXIV.Client.UI.Agent.MapType.GatheringLog;
        mapInfo->MapId = territoryType.Map.Row;
        mapInfo->TerritoryId = territoryType.RowId;
        mapInfo->TitleString = *title;
        agentMap->OpenMap(mapInfo);
        title->Dtor();
        Marshal.FreeHGlobal((nint)titlePtr);

        tooltip->Dtor();
        Marshal.FreeHGlobal((nint)tooltipPtr);
        Marshal.FreeHGlobal((nint)space);

        return true;
    }

    public bool OpenMapWithFishingSpot(FishingSpot? fishingSpot, CachedItem? item = null)
    {
        if (fishingSpot == null)
            return false;

        var territoryType = fishingSpot.TerritoryType.Value;
        if (territoryType == null)
            return false;

        var gatheringItemLevel = 0;
        if (item != null)
        {
            gatheringItemLevel = Service.Data.GetExcelSheet<FishParameter>()
                ?.FirstOrDefault(row => row.Item == (item?.ItemId ?? 0))
                ?.GatheringItemLevel.Value
                ?.GatheringItemLevel ?? 0;
        }

        static int convert(short pos, ushort scale) => (pos - 1024) / (scale / 100);

        var scale = territoryType!.Map.Value!.SizeFactor;
        var x = convert(fishingSpot.X, scale);
        var y = convert(fishingSpot.Z, scale);
        var radius = fishingSpot.Radius / 7 / (scale / 100); // don't ask me why this works

        var raptureTextModule = Framework.Instance()->GetUiModule()->GetRaptureTextModule();

        var levelText = gatheringItemLevel == 0
            ? raptureTextModule->GetAddonText(242) // "Lv. ???"
            : FormatAddonText(raptureTextModule, 35, gatheringItemLevel);

        var tooltip = IMemorySpace.GetDefaultSpace()->Create<Utf8String>();
        tooltip->SetString(levelText);

        var iconId = fishingSpot.Rare ? 60466u : 60465u;

        var agentMap = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
        agentMap->TempMapMarkerCount = 0;
        agentMap->AddGatheringTempMarker(
            4u,
            x,
            y,
            iconId,
            radius,
            tooltip
        );

        var titleBuilder = new SeStringBuilder()
            .Add(new TextPayload("LeveHelper"));
        if (item != null)
        {
            titleBuilder
                .Add(new TextPayload(" ("))
                .AddUiForeground(549)
                .AddUiGlow(550)
                .Add(new TextPayload(item.Name))
                .AddUiGlowOff()
                .AddUiForegroundOff()
                .Add(new TextPayload(")"));
        }
        var titlePtr = MemoryUtils.FromByteArray(titleBuilder.BuiltString.Encode());
        var title = IMemorySpace.GetDefaultSpace()->Create<Utf8String>();
        title->SetString(titlePtr);

        var mapInfo = stackalloc OpenMapInfo[1];
        mapInfo->Type = FFXIVClientStructs.FFXIV.Client.UI.Agent.MapType.GatheringLog;
        mapInfo->MapId = territoryType.Map.Row;
        mapInfo->TerritoryId = territoryType.RowId;
        mapInfo->TitleString = *title;
        agentMap->OpenMap(mapInfo);
        title->Dtor();
        Marshal.FreeHGlobal((nint)titlePtr);

        tooltip->Dtor();

        return true;
    }

    public static bool OpenMapWithMapLink(Level level) => Service.GameGui.OpenMapWithMapLink(GetMapLink(level));
    public static bool OpenMapWithMapLink(TerritoryType territoryType, float x, float y) =>
        Service.GameGui.OpenMapWithMapLink(new MapLinkPayload(territoryType.RowId, territoryType.Map.Row, x, y, 0f));

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
