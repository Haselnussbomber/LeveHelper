using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace LeveHelper.Records;

public record ZoneItems(TerritoryType TerritoryType, HashSet<QueuedItem> Items);
