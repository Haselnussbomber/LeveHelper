using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using LeveHelper.Sheets;

namespace LeveHelper.Filters;

public class TypeFilterConfiguration
{
    public uint SelectedType = 0;
}

public class TypeFilter : Filter
{
    public TypeFilter(FilterManager manager) : base(manager)
    {
    }

    public static TypeFilterConfiguration Config => Plugin.Config.Filters.TypeFilter;

    private Dictionary<string, LeveAssignmentType[]>? Groups = null;

    public override void Reset()
    {
        Config.SelectedType = 0;
    }

    public override bool HasValue()
    {
        return Config.SelectedType != 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedType = (uint)value;
        Plugin.Config.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Type:");

        ImGui.TableNextColumn();
        if (ImGui.BeginCombo("##LeveHelper_TypeFilter_Combo", Config.SelectedType == 0 ? "All" : StringUtil.GetText("LeveAssignmentType", Config.SelectedType, "Unknown")))
        {
            if (ImGui.Selectable("All##LeveHelper_TypeFilter_Combo_0", Config.SelectedType == 0))
            {
                Set(0);
                manager.Update();
            }
            if (Config.SelectedType == 0)
            {
                ImGui.SetItemDefaultFocus();
            }

            ImGuiUtils.DrawIcon(62501, 20, 20);
            ImGui.SameLine();
            if (ImGui.Selectable(StringUtil.GetText("LeveAssignmentType", 1, "Battlecraft") + "##LeveHelper_TypeFilter_Combo_1", Config.SelectedType == 1))
            {
                Set(1);
                manager.Update();
            }
            if (Config.SelectedType == 1)
            {
                ImGui.SetItemDefaultFocus();
            }

            if (Groups == null)
            {
                ImGui.EndCombo(); // LeveHelper_TypeFilter_Combo
                return;
            }

            foreach (var group in Groups)
            {
                ImGui.TextColored(ImGuiUtils.ColorGroup, group.Key);

                foreach (var type in group.Value)
                {
                    var indent = "    ";

                    if (type.Icon != 0)
                    {
                        ImGuiUtils.DrawIcon((uint)type.Icon, 20, 20);
                        ImGui.SameLine();
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
                        indent = "";
                    }

                    if (ImGui.Selectable($"{indent}{type.Name}##LeveHelper_TypeFilter_Combo_{type.RowId}", Config.SelectedType == type.RowId))
                    {
                        Set(type.RowId);
                        manager.Update();
                    }

                    if (Config.SelectedType == type.RowId)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
            }

            ImGui.EndCombo(); // LeveHelper_TypeFilter_Combo
        }

        ImGui.SameLine();

        var suggestedType = Service.ClientState.LocalPlayer?.ClassJob.Id switch
        {
            16 => 2, // Miner
            17 => 3, // Botanist
            18 => 4, // Fisher

            8 => 5, // Carpenter
            9 => 6, // Blacksmith
            10 => 7, // Armorer
            11 => 8, // Goldsmith
            12 => 9, // Leatherworker
            13 => 10, // Weaver
            14 => 11, // Alchemist
            15 => 12, // Culinarian

            _ => 1, // Battlecraft -> any other job
        };

        if (Config.SelectedType != suggestedType)
        {
            var suggestedName = suggestedType == 1
                ? StringUtil.GetText("LeveAssignmentType", 1)
                : Service.ClientState.LocalPlayer?.ClassJob.GameData?.Name?.ToString();
            if (suggestedName != null && ImGui.Button($"Set {suggestedName}"))
            {
                Config.SelectedType = (uint)suggestedType;
                manager.Update();
            }
        }
    }

    private static LeveAssignmentType[] CreateGroup(params uint[] ids)
        => ids
            .Select((rowId) => Service.Data.GetExcelSheet<LeveAssignmentType>()!.GetRow(rowId)!)
            .OrderBy(entry => entry.Name)
            .ToArray();

    public override bool Run()
    {
        Groups ??= new()
        {
            // HowTo#69 => Fieldcraft Leves
            { StringUtil.GetText("HowTo", 69), CreateGroup(2, 3, 4) },

            // HowTo#67 => Tradecraft Leves
            { StringUtil.GetText("HowTo", 67), CreateGroup(5, 6, 7, 8, 9, 10, 11, 12) },

            // HowTo#112 => Grand Company Leves
            { StringUtil.GetText("HowTo", 112), CreateGroup(16, 17, 18) },

            // HowTo#218 => Temple Leves -- no leve is assigned to these
            // { StringUtil.GetText("HowTo", 218), CreateGroup(13, 14, 15) }
        };

        if (Config.SelectedType == 0)
            return false;

        state.Leves = state.Leves.Where(item => item.LeveAssignmentType.Row == Config.SelectedType);

        return true;
    }
}
