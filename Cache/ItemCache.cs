using System.Collections.Generic;

namespace LeveHelper;

public static class ItemCache
{
    public static readonly Dictionary<uint, CachedItem> Cache = new();

    public static CachedItem Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }
}
