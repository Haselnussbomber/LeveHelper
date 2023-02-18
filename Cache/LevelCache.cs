using System.Collections.Generic;

namespace LeveHelper;

public static class LevelCache
{
    public static readonly Dictionary<uint, CachedLevel> Cache = new();

    public static CachedLevel Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var cachedLevel))
            Cache.Add(id, cachedLevel = new(id));

        return cachedLevel;
    }
}
