using System.Collections.Generic;

namespace LeveHelper;

public static class TerritoryTypeCache
{
    public static readonly Dictionary<uint, CachedTerritoryType> Cache = new();

    public static CachedTerritoryType Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }
}
