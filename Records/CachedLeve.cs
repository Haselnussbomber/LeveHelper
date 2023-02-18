using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using LeveHelper.Sheets;

namespace LeveHelper;

public record CachedLeve
{
    public CachedLeve(uint LeveId)
    {
        this.LeveId = LeveId;
    }

    public CachedLeve(Leve leve)
    {
        LeveId = leve.RowId;
        this.leve = leve;
    }

    private Leve? leve = null;
    private string? name = null;
    private string? nameEn = null;
    private string? levemeteName = null;
    private uint? townId = null;
    private string? townName = null;
    private RequiredItem[]? requiredItems = null;
    private CachedLevel? levelLevemete = null;
    private CachedLeveAssignmentType? leveAssignmentType = null;
    private int? leveVfxIcon = null;
    private int? leveVfxFrameIcon = null;

    public uint LeveId { get; private set; }

    public unsafe LeveWork* LeveWork
        => QuestManager.Instance()->GetLeveQuestById((ushort)LeveId);

    public Leve? Leve
        => leve ??= Service.Data.GetExcelSheet<Leve>()?.GetRow(LeveId);

    public string Name
        => name ??= Leve?.Name?.ClearString() ?? $"<{LeveId}>";

    public string NameEn
        => nameEn ??= Service.Data.GetExcelSheet<Leve>(Dalamud.ClientLanguage.English)?.GetRow(LeveId)?.Name?.ClearString() ?? $"<{LeveId}>";

    public ushort ClassJobLevel
        => Leve?.ClassJobLevel ?? 0;

    public int AllowanceCost
        => Leve?.AllowanceCost ?? 0;

    public string? LevemeteName
    {
        get
        {
            if (string.IsNullOrEmpty(levemeteName) && Leve?.LevelLevemete.Value?.Type == 8) // Type 8 = NPC?!?
                levemeteName = Service.GameFunctions.GetENpcResidentName(Leve.LevelLevemete.Value.Object);

            return levemeteName ?? "";
        }
    }

    public CachedLeveAssignmentType? LeveAssignmentType
        => leveAssignmentType ??= leve != null && Leve!.LeveAssignmentType.Value != null ? LeveAssignmentTypeCache.Get(Leve!.LeveAssignmentType.Value) : null;

    public int TypeIcon
        => LeveAssignmentType?.Icon ?? 0;

    public string TypeName
        => LeveAssignmentType?.Name ?? "";

    public uint TownId
        => townId ??= Leve?.Town.Row ?? 0;

    public string TownName
        => townName ??= Leve?.Town.Value?.Name.ClearString() ?? "???";

    public bool TownLocked
        => Leve != null && (Leve.RowId == 546 || Leve.RowId == 556 || Leve.RowId == 566);

    public unsafe bool IsComplete
        => QuestManager.Instance()->IsLevequestComplete((ushort)LeveId);

    public unsafe bool IsAccepted
        => LeveWork != null;

    public unsafe bool IsReadyForTurnIn
        => IsAccepted && LeveWork->Sequence == 255;

    public unsafe bool IsStarted
        => IsAccepted && LeveWork->Sequence == 1 && LeveWork->ClearClass != 0;

    public unsafe bool IsFailed
        => IsAccepted && LeveWork->Sequence == 3;

    public bool IsCraftLeve
        => LeveAssignmentType?.RowId is >= 5 and <= 12;

    public bool IsGatheringLeve
        => LeveAssignmentType?.RowId is >= 2 and <= 4;

    public RequiredItem[] RequiredItems
    {
        get
        {
            if (Leve == null || requiredItems != null)
                return requiredItems ??= Array.Empty<RequiredItem>();

            if (IsCraftLeve)
            {
                var craftLeve = Service.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.CraftLeve>()?.GetRow((uint)Leve.DataId);
                if (craftLeve != null)
                {
                    return requiredItems = craftLeve.UnkData3
                        .Where(item => item.Item != 0 && item.ItemCount != 0)
                        .Aggregate(
                            new Dictionary<int, RequiredItem>(),
                            (dict, entry) =>
                            {
                                if (!dict.TryGetValue(entry.Item, out var reqItem))
                                {
                                    reqItem = new RequiredItem(ItemCache.Get((uint)entry.Item), 0);
                                    dict.Add(entry.Item, reqItem);
                                }

                                reqItem.Amount += entry.ItemCount;

                                return dict;
                            })
                        .Values
                        .ToArray();
                }
            }

            return requiredItems ??= Array.Empty<RequiredItem>();
        }
    }

    public CachedLevel? LevelLevemete
        => levelLevemete ??= Leve != null ? LevelCache.Get(Leve.LevelLevemete.Row) : null;

    public int LeveVfxIcon
        => leveVfxIcon ??= Leve?.LeveVfx.Value?.Icon ?? 0;

    public int LeveVfxFrameIcon
        => leveVfxFrameIcon ??= Leve?.LeveVfxFrame.Value?.Icon ?? 0;
}
