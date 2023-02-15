using System.Collections.Generic;

namespace LeveHelper;

public record ZoneItems(CachedTerritoryType TerritoryType, HashSet<QueuedItem> Items);
