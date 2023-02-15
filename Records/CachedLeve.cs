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
    private string? levemeteName = null;
    private string? typeName = null;
    private string? townName = null;
    private RequiredItem[]? requiredItems = null;

    public uint LeveId { get; private set; }

    public Leve? Leve
        => leve ??= Service.Data.GetExcelSheet<Leve>()?.GetRow(LeveId);

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(name) && Leve != null)
                name = Leve.Name.ClearString();

            return name ?? $"<{LeveId}>";
        }
    }

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

    public string TypeName
    {
        get
        {
            if (string.IsNullOrEmpty(typeName) && Leve != null)
                typeName = Service.Data.GetExcelSheet<LeveAssignmentType>()?.GetRow((uint)Leve.Unknown4)?.Name.ClearString();

            return typeName ?? "";
        }
    }

    public string TownName
    {
        get
        {
            if (string.IsNullOrEmpty(townName) && Leve != null)
                townName = Leve.Town.Value?.Name.ClearString();

            return townName ?? "???";
        }
    }

    public bool TownLocked
        => Leve != null && (Leve.RowId == 546 || Leve.RowId == 556 || Leve.RowId == 566);

    public unsafe bool IsComplete
        => Service.GameFunctions.IsLevequestCompleted(QuestManager.Instance(), (ushort)LeveId); // TODO: remove when client structs is updated

    public bool IsAccepted
        => Service.GameFunctions.IsLevequestAccepted(LeveId);

    public bool IsCraftLeve
        => Leve?.LeveAssignmentType.Row is >= 5 and <= 12;

    public bool IsGatherLeve
        => Leve?.LeveAssignmentType.Row is >= 2 and <= 4;

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
                .Select(item => new RequiredItem(ItemCache.Get((uint)item.Item), item.ItemCount))
                .ToArray();

            return requiredItems;
        }
    }
}
