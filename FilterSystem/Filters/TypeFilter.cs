using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

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

    public static TypeFilterConfiguration Config => Configuration.Instance.Filters.TypeFilter;

    private Dictionary<string, Dictionary<uint, (string, int)>>? Groups = null;

    public override void Reset()
    {
        Config.SelectedType = 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedType = (uint)value;
        Configuration.Save();
    }

    public override void Draw()
    {
        ImGui.TableNextColumn();
        ImGui.Text("Type:");

        ImGui.TableNextColumn();
        if (!ImGui.BeginCombo("##LeveHelper_TypeFilter_Combo", Config.SelectedType == 0 ? "All" : StringUtil.GetText("LeveAssignmentType", Config.SelectedType, "Unknown")))
        {
            return;
        }

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

                if (type.Value.Item2 != 0)
                {
                    ImGuiUtils.DrawIcon(type.Value.Item2, 20, 20);
                    ImGui.SameLine();
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
                    indent = "";
                }

                if (ImGui.Selectable($"{indent}{type.Value.Item1}##LeveHelper_TypeFilter_Combo_{type.Key}", Config.SelectedType == type.Key))
                {
                    Set(type.Key);
                    manager.Update();
                }

                if (Config.SelectedType == type.Key)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
        }

        ImGui.EndCombo(); // LeveHelper_TypeFilter_Combo
    }

    private Dictionary<uint, (string, int)> CreateGroup(params uint[] ids)
    {
        var sheet = Service.Data.GetExcelSheet<LeveAssignmentType>();
        return ids
            .Select(key => (Id: key, Icon: sheet?.GetRow(key)?.Icon ?? 0, Title: StringUtil.GetText("LeveAssignmentType", key)))
            .OrderBy(entry => entry.Title)
            .ToDictionary(entry => entry.Id, entry => (entry.Title, entry.Icon));
    }

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
        {
            return false;
        }

        state.Leves = state.Leves.Where(item => item.leve.Unknown4 == Config.SelectedType);

        return true;
    }
}
