using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record LeveRecord
{
    public readonly Leve leve;
    public readonly string RowId;
    public readonly string Name;
    public readonly string ClassJobLevel;
    public readonly string? LevemeteName;
    public readonly string ClassName;
    public readonly string TownName;
    public readonly bool TownLocked;

    public LeveRecord(Leve leve)
    {
        this.leve = leve;
        this.RowId = leve.RowId.ToString();
        this.Name = leve.Name.ClearString();
        this.ClassJobLevel = leve.ClassJobLevel.ToString();

        var levemeteLevel = leve.LevelLevemete.Value;
        if (levemeteLevel?.Type == 8) // Type: NPC?!?
        {
            var npc = Service.Data.GetExcelSheet<ENpcResident>()!.GetRow(levemeteLevel.Object);
            this.LevemeteName = npc?.Singular.ClearString();
        }

        this.ClassName = Service.Data.GetExcelSheet<LeveAssignmentType>()!.GetRow((uint)leve.Unknown4)!.Name.ClearString();
        this.TownName = leve.Town.Value?.Name.ClearString() ?? "???";
        this.TownLocked = leve.RowId == 546 || leve.RowId == 556 || leve.RowId == 566;
    }

    public bool IsComplete => QuestManagerHelper.Instance.IsLevequestCompleted((ushort)leve.RowId);
}
