using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class NpcCache
{
    public static readonly Dictionary<uint, CachedNpc> Cache = new();
    public static readonly Dictionary<uint, List<CachedNpc>> GilShopCache = new();

    public static CachedNpc? Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var npc))
            Cache.Add(id, npc = new(id));

        return npc;
    }

    public static List<CachedNpc> GetByGilShop(uint shopId)
    {
        // TODO: huge performance problem
        if (!GilShopCache.TryGetValue(shopId, out var npcs))
        {
            npcs = Service.Data.GetExcelSheet<ENpcBase>()?
                .Where(row => row.ENpcData.Any(data => data == shopId))
                .Select(row => Get(row.RowId))
                .Where(npc => npc != null)
                .Cast<CachedNpc>()
                .ToList();

            GilShopCache.Add(shopId, npcs!);
        }

        return npcs ?? new();
    }
}

public record CachedNpc
{
    public CachedNpc(uint npcId)
    {
        NpcId = npcId;
    }

    private string name = "";
    private ENpcResident? eNpcResident = null;
    private Level? level = null;

    public uint NpcId { get; init; }

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(name))
                name = Service.GameFunctions.GetENpcResidentName(NpcId);

            return string.IsNullOrEmpty(name)
                ? Npc?.Singular.ClearString() ?? ""
                : name;
        }
    }

    public ENpcResident? Npc
        => eNpcResident ??= Service.Data.GetExcelSheet<ENpcResident>()?.GetRow(NpcId);

    public Level? Level
        => level ??= Service.Data.GetExcelSheet<Level>()?.FirstOrDefault(row => row.Type == 8 && row.Object == NpcId);
}
