using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace LeveHelper.Sheets;

public class Leve : Lumina.Excel.GeneratedSheets.Leve
{
    private string? _name = null;
    private string? _levemeteName = null;
    private string? _townName = null;
    private RequiredItem[]? _requiredItems = null;

    public unsafe LeveWork* LeveWork
        => QuestManager.Instance()->GetLeveQuestById((ushort)RowId);

    public new string Name
        => _name ??= base.Name?.ToDalamudString().ToString() ?? $"<{RowId}>";

    public string? LevemeteName
    {
        get
        {
            if (string.IsNullOrEmpty(_levemeteName) && base.LevelLevemete.Value?.Type == 8) // Type 8 = NPC?!?
                _levemeteName = Service.GameFunctions.GetENpcResidentName(base.LevelLevemete.Value.Object);

            return _levemeteName ?? "";
        }
    }

    public int TypeIcon
        => LeveAssignmentType.Value?.Icon ?? 0;

    public string TypeName
        => LeveAssignmentType.Value?.Name ?? "";

    public string TownName
        => _townName ??= Town.Value?.Name.ToDalamudString().ToString() ?? "???";

    public bool TownLocked
        => RowId == 546 || RowId == 556 || RowId == 566;

    public unsafe bool IsComplete
        => QuestManager.Instance()->IsLevequestComplete((ushort)RowId);

    public unsafe bool IsAccepted
        => LeveWork != null;

    public unsafe bool IsReadyForTurnIn
        => IsAccepted && LeveWork->Sequence == 255;

    public unsafe bool IsStarted
        => IsAccepted && LeveWork->Sequence == 1 && LeveWork->ClearClass != 0;

    public unsafe bool IsFailed
        => IsAccepted && LeveWork->Sequence == 3;

    public bool IsCraftLeve
        => LeveAssignmentType.Row is >= 5 and <= 12;

    public RequiredItem[] RequiredItems
    {
        get
        {
            if (_requiredItems != null)
                return _requiredItems ??= Array.Empty<RequiredItem>();

            if (IsCraftLeve)
            {
                var craftLeve = Service.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.CraftLeve>()?.GetRow((uint)DataId);
                if (craftLeve != null)
                {
                    return _requiredItems = craftLeve.UnkData3
                        .Where(item => item.Item != 0 && item.ItemCount != 0)
                        .Aggregate(
                            new Dictionary<int, RequiredItem>(),
                            (dict, entry) =>
                            {
                                if (!dict.TryGetValue(entry.Item, out var reqItem))
                                {
                                    reqItem = new RequiredItem(Service.Data.GetExcelSheet<Item>()!.GetRow((uint)entry.Item)!, 0);
                                    dict.Add(entry.Item, reqItem);
                                }

                                reqItem.Amount += entry.ItemCount;

                                return dict;
                            })
                        .Values
                        .ToArray();
                }
            }

            return _requiredItems ??= Array.Empty<RequiredItem>();
        }
    }
}
