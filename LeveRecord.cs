using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record LeveRecord
{
    public readonly Leve leve;
    public readonly string RowId;
    public readonly string Name;
    public readonly string ClassJobLevel;
    public readonly string? LevemeteName;
    public readonly string TypeName;
    public readonly string TownName;
    public readonly bool TownLocked;

    public LeveRecord(Leve leve)
    {
        this.leve = leve;
        RowId = leve.RowId.ToString();
        Name = leve.Name.ClearString();
        ClassJobLevel = leve.ClassJobLevel.ToString();

        var levemeteLevel = leve.LevelLevemete.Value;
        if (levemeteLevel?.Type == 8) // Type: NPC?!?
        {
            LevemeteName = Service.GameFunctions.GetENpcResidentName(levemeteLevel.Object);
        }

        TypeName = Service.Data.GetExcelSheet<LeveAssignmentType>()!.GetRow((uint)leve.Unknown4)!.Name.ClearString();
        TownName = leve.Town.Value?.Name.ClearString() ?? "???";
        TownLocked = leve.RowId == 546 || leve.RowId == 556 || leve.RowId == 566;
    }

    public unsafe bool IsComplete => Service.GameFunctions.IsLevequestCompleted(QuestManager.Instance(), (ushort)leve.RowId);
}
