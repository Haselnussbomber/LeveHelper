#pragma warning disable 0649
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using LeveHelper.Sheets;
using LeveHelper.Utils;
using ExportedGatheringPoint = Lumina.Excel.GeneratedSheets.ExportedGatheringPoint;
using FishParameter = Lumina.Excel.GeneratedSheets.FishParameter;
using Level = Lumina.Excel.GeneratedSheets.Level;
using MapType = FFXIVClientStructs.FFXIV.Client.UI.Agent.MapType;

namespace LeveHelper;

public unsafe class GameFunctions
{
    private readonly Dictionary<uint, string> _eNpcResidentNameCache = new();

    public GameFunctions()
    {
        SignatureHelper.Initialise(this);
    }

    public ushort[] ActiveLevequestsIds
    {
        get
        {
            var ids = new List<ushort>();

            foreach (var entry in QuestManager.Instance()->LeveQuestsSpan)
            {
                if (entry.LeveId != 0)
                    ids.Add(entry.LeveId);
            }

            return ids.ToArray();
        }
    }

    public Leve[] ActiveLevequests
    {
        get
        {
            var ids = new List<Leve>();

            foreach (var entry in QuestManager.Instance()->LeveQuestsSpan)
            {
                if (entry.LeveId != 0)
                    ids.Add(Service.DataManager.GetExcelSheet<Leve>()!.GetRow(entry.LeveId)!);
            }

            return ids.ToArray();
        }
    }

    [Signature("80 F9 07 77 10")]
    public readonly IsGatheringPointTypeOffDelegate IsGatheringPointRare = null!;
    public delegate bool IsGatheringPointTypeOffDelegate(byte gatheringPointType);

    [Signature("E8 ?? ?? ?? ?? 4C 8B 05 ?? ?? ?? ?? 48 8D 8C 24 ?? ?? ?? ?? 48 8B D0 E8 ?? ?? ?? ?? 8B 4E 08")]
    public readonly GetGatheringPointNameDelegate GetGatheringPointName = null!;
    public delegate byte* GetGatheringPointNameDelegate(RaptureTextModule** module, byte gatheringType, byte gatheringPointType);

    [Signature("E8 ?? ?? ?? ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 30 5B C3 48 8B CB")]
    public readonly AgentJournal_OpenForQuestDelegate AgentJournal_OpenForQuest = null!;
    public delegate byte* AgentJournal_OpenForQuestDelegate(nint agentJournal, int id, int type, ushort a4 = 0, bool a5 = false); // type: 1 = Quest, 2 = Levequest

    public string GetENpcResidentName(uint npcId)
    {
        if (!_eNpcResidentNameCache.TryGetValue(npcId, out var name))
        {
            var textPtr = RaptureTextModule.Instance()->FormatAddonText2(2025, (int)npcId, 1);
            name = MemoryHelper.ReadSeStringNullTerminated((nint)textPtr).ToString();
            _eNpcResidentNameCache.Add(npcId, name);
        }

        return name;
    }

    public bool OpenMapWithGatheringPoint(GatheringPoint? gatheringPoint, Item? item = null)
    {
        var agentMap = AgentMap.Instance();
        if (agentMap == null)
            return false;

        if (gatheringPoint == null)
            return false;

        var territoryType = gatheringPoint.TerritoryType.Value;
        if (territoryType == null)
            return false;

        var gatheringPointBase = gatheringPoint.GatheringPointBase.Value;
        if (gatheringPointBase == null)
            return false;

        var exportedPoint = Service.DataManager.GetExcelSheet<ExportedGatheringPoint>()?.GetRow(gatheringPointBase.RowId);
        if (exportedPoint == null)
            return false;

        var gatheringType = exportedPoint.GatheringType.Value;
        if (gatheringType == null)
            return false;

        var raptureTextModule = RaptureTextModule.Instance();

        var levelText = gatheringPointBase.GatheringLevel == 1
            ? raptureTextModule->GetAddonText(242) // "Lv. ???"
            : raptureTextModule->FormatAddonText2(35, gatheringPointBase.GatheringLevel, 0);
        var gatheringPointName = GetGatheringPointName(
            &raptureTextModule,
            (byte)exportedPoint.GatheringType.Row,
            exportedPoint.GatheringPointType
        );

        using var tooltip = new DisposableUtf8String(levelText);
        tooltip.AppendString(" ");
        tooltip.AppendString(gatheringPointName);

        var iconId = !IsGatheringPointRare(exportedPoint.GatheringPointType)
            ? gatheringType.IconMain
            : gatheringType.IconOff;

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

        using var title = new DisposableUtf8String(titleBuilder.BuiltString);

        var mapInfo = stackalloc OpenMapInfo[1];
        mapInfo->Type = MapType.GatheringLog;
        mapInfo->MapId = territoryType.Map.Row;
        mapInfo->TerritoryId = territoryType.RowId;
        mapInfo->TitleString = *title.Ptr;
        agentMap->OpenMap(mapInfo);

        return true;
    }

    public bool OpenMapWithFishingSpot(FishingSpot? fishingSpot, Item? item = null)
    {
        var agentMap = AgentMap.Instance();
        if (agentMap == null)
            return false;

        if (fishingSpot == null)
            return false;

        var territoryType = fishingSpot.TerritoryType.Value;
        if (territoryType == null)
            return false;

        var gatheringItemLevel = 0;
        if (item != null)
        {
            gatheringItemLevel = Service.DataManager.GetExcelSheet<FishParameter>()
                ?.FirstOrDefault(row => row.Item == (item?.RowId ?? 0))
                ?.GatheringItemLevel.Value
                ?.GatheringItemLevel ?? 0;
        }

        static int convert(short pos, ushort scale) => (pos - 1024) / (scale / 100);

        var scale = territoryType!.Map.Value!.SizeFactor;
        var x = convert(fishingSpot.X, scale);
        var y = convert(fishingSpot.Z, scale);
        var radius = fishingSpot.Radius / 7 / (scale / 100); // don't ask me why this works

        var raptureTextModule = RaptureTextModule.Instance();

        var levelText = gatheringItemLevel == 0
            ? raptureTextModule->GetAddonText(242) // "Lv. ???"
            : raptureTextModule->FormatAddonText2(35, gatheringItemLevel, 0);

        using var tooltip = new DisposableUtf8String(levelText);

        var iconId = fishingSpot.Rare ? 60466u : 60465u;

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

        using var title = new DisposableUtf8String(titleBuilder.BuiltString);

        var mapInfo = stackalloc OpenMapInfo[1];
        mapInfo->Type = MapType.GatheringLog;
        mapInfo->MapId = territoryType.Map.Row;
        mapInfo->TerritoryId = territoryType.RowId;
        mapInfo->TitleString = *title.Ptr;
        agentMap->OpenMap(mapInfo);

        return true;
    }

    public static bool OpenMapWithMapLink(Level level)
        => Service.GameGui.OpenMapWithMapLink(GetMapLink(level));
    public static bool OpenMapWithMapLink(TerritoryType territoryType, float x, float y)
        => Service.GameGui.OpenMapWithMapLink(new MapLinkPayload(territoryType.RowId, territoryType.Map.Row, x, y, 0f));

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
