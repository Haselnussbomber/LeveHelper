using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class LeveAssignmentTypeCache
{
    public static readonly Dictionary<uint, CachedLeveAssignmentType> Cache = new();

    public static CachedLeveAssignmentType Get(uint id)
    {
        if (!Cache.TryGetValue(id, out var cachedLeveAssignmentType))
            Cache.Add(id, cachedLeveAssignmentType = new(id));

        return cachedLeveAssignmentType;
    }

    public static CachedLeveAssignmentType Get(LeveAssignmentType leveAssignmentType)
    {
        if (!Cache.TryGetValue(leveAssignmentType.RowId, out var cachedLeveAssignmentType))
            Cache.Add(leveAssignmentType.RowId, cachedLeveAssignmentType = new(leveAssignmentType));

        return cachedLeveAssignmentType;
    }
}
