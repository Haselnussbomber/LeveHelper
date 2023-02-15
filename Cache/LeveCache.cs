using System.Collections.Generic;
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
