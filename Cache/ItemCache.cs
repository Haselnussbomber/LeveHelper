using System.Collections.Generic;

namespace LeveHelper;

public static class ItemCache
{
    public static readonly Dictionary<uint, CachedItem> Cache = new();

    public static CachedItem Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var cachedItem))
            Cache.Add(id, cachedItem = new(id));

        return cachedItem;
    }
}
