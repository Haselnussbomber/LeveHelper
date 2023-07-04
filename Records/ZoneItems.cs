using System.Collections.Generic;
using LeveHelper.Sheets;

namespace LeveHelper;

public record ZoneItems(TerritoryType TerritoryType, HashSet<QueuedItem> Items);
