using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using HaselCommon.Utils;
using LeveHelper.Sheets;

namespace LeveHelper.Utils;

public static unsafe class QuestUtils
{
    private enum QuestType : uint
    {
        NormalQuest = 1,
        LeveQuest = 2,
    }

    private delegate void OpenJournalDelegate(nint agentQuestJournal, uint id, QuestType type, ushort a4 = 0, bool toggle = false);
    private static OpenJournalDelegate _openJournal { get; set; } = MemoryUtils.GetDelegateForSignature<OpenJournalDelegate>("E8 ?? ?? ?? ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 30 5B C3 48 8B CB");

    public static void OpenJournal(uint id)
    {
        var agent = GetAgent<AgentInterface>(AgentId.Journal);
        _openJournal((nint)agent, id, QuestType.LeveQuest);
    }

    public static IEnumerable<ushort> GetActiveLeveIds()
        => QuestManager.Instance()->LeveQuestsSpan
            .ToArray()
            .Where(entry => entry.LeveId != 0)
            .Select(entry => entry.LeveId);

    public static IEnumerable<Leve> GetActiveLeves()
        => GetActiveLeveIds().Select(leveId => GetRow<Leve>(leveId)!);
}
