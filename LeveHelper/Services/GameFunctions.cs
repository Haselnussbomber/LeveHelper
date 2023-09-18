using System.Collections.Generic;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using LeveHelper.Sheets;

namespace LeveHelper.Services;

public unsafe class GameFunctions
{
    public GameFunctions()
    {
        Service.GameInteropProvider.InitializeFromAttributes(this);
    }

    public ushort[] ActiveLevequestsIds
    {
        get
        {
            var ids = new List<ushort>();

            foreach (var entry in QuestManager.Instance()->LeveQuestsSpan)
            {
                if (entry.LeveId != 0)
                    ids.Add(entry.LeveId);
            }

            return ids.ToArray();
        }
    }

    public Leve[] ActiveLevequests
    {
        get
        {
            var ids = new List<Leve>();

            foreach (var entry in QuestManager.Instance()->LeveQuestsSpan)
            {
                if (entry.LeveId != 0)
                    ids.Add(GetRow<Leve>(entry.LeveId)!);
            }

            return ids.ToArray();
        }
    }

    [Signature("E8 ?? ?? ?? ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 30 5B C3 48 8B CB")]
    public readonly AgentJournal_OpenForQuestDelegate AgentJournal_OpenForQuest = null!;
    public delegate byte* AgentJournal_OpenForQuestDelegate(nint agentJournal, int id, int type, ushort a4 = 0, bool a5 = false); // type: 1 = Quest, 2 = Levequest
}
