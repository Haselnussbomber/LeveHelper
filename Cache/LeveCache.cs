using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class LeveCache
{
    public static readonly Dictionary<uint, CachedLeve> Cache = new();

    public static CachedLeve Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var leve))
            Cache.Add(id, leve = new(id));

        return leve;
    }

    public static CachedLeve Get(Leve leve)
    {
        if (!Cache.TryGetValue(leve.RowId, out var cachedLeve))
            Cache.Add(leve.RowId, cachedLeve = new(leve));

        return cachedLeve;
    }
}

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

public record RequiredItem
{
    public RequiredItem(CachedItem Item, uint Amount)
    {
        this.Item = Item;
        this.Amount = Amount;
    }

    public CachedItem Item { get; init; }
    public uint Amount { get; set; }
}

public record QueuedItem
{
    public QueuedItem(CachedItem Item, uint Amount)
    {
        this.Item = Item;
        AmountTotal = Amount;
        AmountLeft = Amount;
    }

    public CachedItem Item { get; init; }
    public uint AmountTotal { get; set; }
    public uint AmountLeft { get; set; }

    public void AddAmount(uint amount)
    {
        AmountTotal += amount;
        AmountLeft += amount;
    }
}
