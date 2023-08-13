using System.Collections.Generic;
using System.Linq;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace LeveHelper.Sheets;

public class GatheringItem : Lumina.Excel.GeneratedSheets.GatheringItem
{
    private GatheringPoint[]? _gatheringPoints = null;

    public LazyRow<Item> ItemRow { get; set; } = null!;

    public override void PopulateData(RowParser parser, GameData gameData, Language language)
    {
        base.PopulateData(parser, gameData, language);

        ItemRow = new LazyRow<Item>(gameData, Item, language);
    }

    public GatheringPoint[] GatheringPoints
    {
        get
        {
            if (_gatheringPoints != null)
                return _gatheringPoints;

            var GatheringPointSheet = GetSheet<GatheringPoint>();

            //! https://github.com/Ottermandias/GatherBuddy/blob/56da5c9/GatherBuddy.GameData/Data/HiddenItems.cs
            var pointBases = Item switch
            {
                7758 => new() { 203 },  // Grade 1 La Noscean Topsoil
                7761 => new() { 200 },  // Grade 1 Shroud Topsoil
                7764 => new() { 201 },  // Grade 1 Thanalan Topsoil
                7759 => new() { 150 },  // Grade 2 La Noscean Topsoil
                7762 => new() { 209 },  // Grade 2 Shroud Topsoil
                7765 => new() { 151 },  // Grade 2 Thanalan Topsoil
                10092 => new() { 210 }, // Black Limestone
                10094 => new() { 177 }, // Little Worm
                10097 => new() { 133 }, // Yafaemi Wildgrass
                12893 => new() { 295 }, // Dark Chestnut
                15865 => new() { 30 },  // Firelight Seeds
                15866 => new() { 39 },  // Icelight Seeds
                15867 => new() { 21 },  // Windlight Seeds
                15868 => new() { 31 },  // Earthlight Seeds
                15869 => new() { 25 },  // Levinlight Seeds
                15870 => new() { 14 },  // Waterlight Seeds
                12534 => new() { 285 }, // Mythrite Ore
                12535 => new() { 353 }, // Hardsilver Ore
                12537 => new() { 286 }, // Titanium Ore
                12579 => new() { 356 }, // Birch Log
                12878 => new() { 297 }, // Cyclops Onion
                12879 => new() { 298 }, // Emerald Beans
                _ => new HashSet<uint>()
            };

            foreach (var point in GatheringPointSheet)
            {
                if (point.TerritoryType.Row <= 1)
                    continue;

                if (point.GatheringPointBase.Value == null)
                    continue;

                var gatheringPointBase = point.GatheringPointBase.Value;

                // only accept Mining, Quarrying, Logging and Harvesting
                if (gatheringPointBase.GatheringType.Row >= 5)
                    continue;

                foreach (var gatheringItemId in gatheringPointBase.Item)
                {
                    if (gatheringItemId == RowId)
                    {
                        pointBases.Add(gatheringPointBase.RowId);
                    }
                }
            }

            return _gatheringPoints ??= pointBases
                .Select((baseId) => GatheringPointSheet.Where((row) => row.TerritoryType.Row > 1 && row.GatheringPointBase.Row == baseId))
                .SelectMany(e => e)
                .OfType<GatheringPoint>().ToArray();
        }
    }
}
