using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record ZoneItems(TerritoryType TerritoryType, HashSet<QueuedItem> Items);
