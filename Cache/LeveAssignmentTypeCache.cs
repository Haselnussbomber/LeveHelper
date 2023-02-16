using System.Collections.Generic;

namespace LeveHelper;

public static class LeveAssignmentTypeCache
{
    public static readonly Dictionary<uint, CachedLeveAssignmentType> Cache = new();

    public static CachedLeveAssignmentType Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var item))
            Cache.Add(id, item = new(id));

        return item;
    }
}
