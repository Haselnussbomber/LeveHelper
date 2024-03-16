using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using LeveHelper.Sheets;

namespace LeveHelper.Utils;

public static unsafe class QuestUtils
{
    public static IEnumerable<ushort> GetActiveLeveIds()
    {
        var leveIds = new HashSet<ushort>();

        foreach (ref var entry in QuestManager.Instance()->LeveQuestsSpan)
        {
            if (entry.LeveId != 0)
                leveIds.Add(entry.LeveId);
        }

        return leveIds;
    }

    public static IEnumerable<Leve> GetActiveLeves()
    {
        var leves = new HashSet<Leve>();
        Leve? leve;

        foreach (var leveId in GetActiveLeveIds())
        {
            if ((leve = GetRow<Leve>(leveId)) != null)
                leves.Add(leve);
        }

        return leves;
    }
}
