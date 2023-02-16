using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

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

    public Leve? Leve
        => leve ??= Service.Data.GetExcelSheet<Leve>()?.GetRow(LeveId);

    public string Name
        => name ??= Leve?.Name.ClearString() ?? $"<{LeveId}>";

    public string NameEn
        => nameEn ??= Service.Data.GetExcelSheet<Leve>(Dalamud.ClientLanguage.English)?.GetRow(LeveId)?.Name.ClearString() ?? $"<{LeveId}>";

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
        => leveAssignmentType ??= leve != null ? LeveAssignmentTypeCache.Get((uint)Leve!.Unknown4) : null; // leve.LeveAssignmentType.Row seems to have moved

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
        => Service.GameFunctions.IsLevequestCompleted(QuestManager.Instance(), (ushort)LeveId); // TODO: remove when client structs is updated

    public bool IsAccepted
        => Service.GameFunctions.IsLevequestAccepted(LeveId);

    public bool IsCraftLeve
        => LeveAssignmentType?.RowId is >= 5 and <= 12;

    public bool IsGatherLeve
        => LeveAssignmentType?.RowId is >= 2 and <= 4;

    public RequiredItem[]? RequiredItems
    {
        get
        {
            if (!IsCraftLeve || Leve == null)
                return null;

            var craftLeve = Service.Data.GetExcelSheet<CraftLeve>()?.GetRow((uint)Leve.DataId);
            if (craftLeve == null)
                return null;

            requiredItems = craftLeve.UnkData3
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

            return requiredItems;
        }
    }

    public CachedLevel? LevelLevemete
        => levelLevemete ??= Leve != null ? LevelCache.Get(Leve.LevelLevemete.Row) : null;

    public int LeveVfxIcon
        => leveVfxIcon ??= Leve?.LeveVfx.Value?.Icon ?? 0;

    public int LeveVfxFrameIcon
        => leveVfxFrameIcon ??= Leve?.LeveVfxFrame.Value?.Icon ?? 0;
}
