using System.Collections.Generic;

namespace LeveHelper;

public static class LevelCache
{
    public static readonly Dictionary<uint, CachedLevel> Cache = new();

    public static CachedLevel Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }
}
