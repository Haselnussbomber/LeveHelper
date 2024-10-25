using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Config;
using LeveHelper.Interfaces;
using LeveHelper.Utils;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Filters;

public class TypeFilterConfiguration
{
    public uint SelectedType = 0;
}

public class TypeFilter(
    IClientState ClientState,
    PluginConfig PluginConfig,
    TextService TextService,
    ExcelService ExcelService,
    TextureService TextureService) : IFilter
{
    public int Order => 2;
    public TypeFilterConfiguration Config => PluginConfig.Filters.TypeFilter;

    public FilterManager? FilterManager { get; set; }

    private Dictionary<string, LeveAssignmentType[]> Groups { get; set; } = [];

    public void Reload()
    {
        Groups.Clear();
        Run();
    }

    public void Reset()
    {
        Config.SelectedType = 0;
    }

    public bool HasValue()
    {
        return Config.SelectedType != 0;
    }

    public void Set(dynamic value)
    {
        Config.SelectedType = (uint)value;
        PluginConfig.Save();
    }

    public void Draw()
    {
        using var id = ImRaii.PushId("TypeFilter");

        ImGui.TableNextColumn();
        TextService.Draw("TypeFilter.Label");

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(250);
        var preview = Config.SelectedType == 0
            ? TextService.Translate("TypeFilter.Selectable.All")
            : ExcelService.GetRow<LeveAssignmentType>(Config.SelectedType)?.Name.AsReadOnly().ExtractText() ?? string.Empty;
        using (var combo = ImRaii.Combo("##Combo", preview))
        {
            if (combo)
            {
                if (ImGui.Selectable(TextService.Translate("TypeFilter.Selectable.All") + "##All", Config.SelectedType == 0))
                {
                    Set(0);
                    FilterManager!.Update();
                }
                if (Config.SelectedType == 0)
                {
                    ImGui.SetItemDefaultFocus();
                }

                TextureService.DrawIcon(62501, 20);
                ImGui.SameLine();
                if (ImGui.Selectable(ExcelService.GetRow<LeveAssignmentType>(1)!.Name.AsReadOnly().ExtractText() + "##Battlecraft", Config.SelectedType == 1))
                {
                    Set(1);
                    FilterManager!.Update();
                }
                if (Config.SelectedType == 1)
                {
                    ImGui.SetItemDefaultFocus();
                }

                foreach (var group in Groups)
                {
                    ImGui.TextColored(LeveHelperColors.Group, group.Key);

                    foreach (var type in group.Value)
                    {
                        var indent = "    ";

                        if (type.Icon != 0)
                        {
                            TextureService.DrawIcon(type.Icon, 20);
                            ImGui.SameLine();
                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
                            indent = "";
                        }

                        if (ImGui.Selectable($"{indent}{type.Name}##Entry_{type.RowId}", Config.SelectedType == type.RowId))
                        {
                            Set(type.RowId);
                            FilterManager!.Update();
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

        var suggestedType = ClientState.LocalPlayer?.ClassJob.Id switch
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
                ? ExcelService.GetRow<LeveAssignmentType>(1)!.Name.AsReadOnly().ExtractText()
                : ClientState.LocalPlayer?.ClassJob.GameData?.Name?.ToString();
            if (suggestedName != null && ImGui.Button(TextService.Translate("TypeFilter.SetSuggestion", suggestedName)))
            {
                Set((uint)suggestedType);
                FilterManager!.Update();
            }
        }
    }

    private LeveAssignmentType[] CreateGroup(params uint[] ids)
        => ids
            .Select((rowId) => ExcelService.GetRow<LeveAssignmentType>(rowId))
            .OfType<LeveAssignmentType>()
            .OrderBy(entry => entry.Name.AsReadOnly().ExtractText())
            .ToArray();

    public bool Run()
    {
        if (Groups.Count == 0)
        {
            // HowTo#69 => Fieldcraft Leves
            Groups.Add(ExcelService.GetRow<HowTo>(69)!.Name.AsReadOnly().ExtractText(), CreateGroup(2, 3, 4));

            // HowTo#67 => Tradecraft Leves
            Groups.Add(ExcelService.GetRow<HowTo>(67)!.Name.AsReadOnly().ExtractText(), CreateGroup(5, 6, 7, 8, 9, 10, 11, 12));

            // HowTo#112 => Grand Company Leves
            Groups.Add(ExcelService.GetRow<HowTo>(112)!.Name.AsReadOnly().ExtractText(), CreateGroup(16, 17, 18));

            // HowTo#218 => Temple Leves -- no leve is assigned to these
            //_groups.Add(ExcelService.GetRow<HowTo>(218)!.Name.ExtractText(), CreateGroup(13, 14, 15));
        }

        if (Config.SelectedType == 0)
            return false;

        FilterManager!.State.Leves = FilterManager.State.Leves.Where(item => item.LeveAssignmentType.Row == Config.SelectedType);

        return true;
    }
}
