using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using LeveHelper.Sheets;

namespace LeveHelper.Utils;

public static unsafe class QuestUtils
{
    public static IEnumerable<ushort> GetActiveLeveIds()
        => QuestManager.Instance()->LeveQuestsSpan
            .ToArray()
            .Where(entry => entry.LeveId != 0)
            .Select(entry => entry.LeveId);

    public static IEnumerable<Leve> GetActiveLeves()
        => GetActiveLeveIds().Select(leveId => GetRow<Leve>(leveId)!);
}
