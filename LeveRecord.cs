using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record LeveRecord
{
    public readonly Leve value;
    public readonly string RowId;
    public readonly string Name;
    public readonly string ClassJobLevel;
    public readonly string? LevemeteName;
    public readonly string ClassName;
    public readonly string TownName;
    public readonly bool TownLocked;

    public LeveRecord(Leve leve)
    {
        this.value = leve;
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
        this.TownLocked = value.RowId == 546 || value.RowId == 556 || value.RowId == 566;
    }

    public bool IsComplete => QuestManagerHelper.Instance.IsLevequestCompleted((ushort)value.RowId);
}
