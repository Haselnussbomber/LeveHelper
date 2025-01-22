using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace LeveHelper;

public record ZoneItems(TerritoryType TerritoryType, HashSet<QueuedItem> Items);
