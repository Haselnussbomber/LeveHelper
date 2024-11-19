using System.Collections.Generic;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LeveHelper;

public record ZoneItems(RowRef<TerritoryType> TerritoryType, HashSet<QueuedItem> Items);
