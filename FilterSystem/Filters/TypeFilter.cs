using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Raii;
using ImGuiNET;
using LeveHelper.Utils;
using Lumina.Excel.GeneratedSheets;
using LeveAssignmentType = LeveHelper.Sheets.LeveAssignmentType;

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

    private Dictionary<string, LeveAssignmentType[]>? _groups { get; set; }

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
        using var id = ImRaii.PushId("TypeFilter");

        ImGui.TableNextColumn();
        ImGui.Text("Type:");

        ImGui.TableNextColumn();
        using (var combo = ImRaii.Combo("##Combo", Config.SelectedType == 0 ? "All" : GetSheetText<LeveAssignmentType>(Config.SelectedType, "Name")))
        {
            if (combo.Success)
            {
                if (ImGui.Selectable("All##All", Config.SelectedType == 0))
                {
                    Set(0);
                    manager.Update();
                }
                if (Config.SelectedType == 0)
                {
                    ImGui.SetItemDefaultFocus();
                }

                Service.TextureManager.GetIcon(62501).Draw(20);
                ImGui.SameLine();
                if (ImGui.Selectable(GetSheetText<LeveAssignmentType>(1, "Name") + "##Battlecraft", Config.SelectedType == 1))
                {
                    Set(1);
                    manager.Update();
                }
                if (Config.SelectedType == 1)
                {
                    ImGui.SetItemDefaultFocus();
                }

                if (_groups == null)
                {
                    ImGui.EndCombo(); // Combo
                    return;
                }

                foreach (var group in _groups)
                {
                    ImGui.TextColored(Colors.Group, group.Key);

                    foreach (var type in group.Value)
                    {
                        var indent = "    ";

                        if (type.Icon != 0)
                        {
                            Service.TextureManager.GetIcon(type.Icon).Draw(20);
                            ImGui.SameLine();
                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
                            indent = "";
                        }

                        if (ImGui.Selectable($"{indent}{type.Name}##Entry_{type.RowId}", Config.SelectedType == type.RowId))
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
            }
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
                ? GetSheetText<LeveAssignmentType>(1, "Name")
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
            .Select((rowId) => GetRow<LeveAssignmentType>(rowId)!)
            .OrderBy(entry => entry.Name)
            .ToArray();

    public override bool Run()
    {
        _groups ??= new()
        {
            // HowTo#69 => Fieldcraft Leves
            { GetSheetText<HowTo>(69, "Name"), CreateGroup(2, 3, 4) },

            // HowTo#67 => Tradecraft Leves
            { GetSheetText<HowTo>(67, "Name"), CreateGroup(5, 6, 7, 8, 9, 10, 11, 12) },

            // HowTo#112 => Grand Company Leves
            { GetSheetText<HowTo>(112, "Name"), CreateGroup(16, 17, 18) },

            // HowTo#218 => Temple Leves -- no leve is assigned to these
            // { StringUtil.GetText("HowTo", 218), CreateGroup(13, 14, 15) }
        };

        if (Config.SelectedType == 0)
            return false;

        state.Leves = state.Leves.Where(item => item.LeveAssignmentType.Row == Config.SelectedType);

        return true;
    }
}
