using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Raii;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Filters;

public class LevemeteFilterConfiguration
{
    public uint SelectedLevemete = 0;
}

public class LevemeteFilter : Filter
{
    public LevemeteFilter(FilterManager manager) : base(manager)
    {
    }

    public static LevemeteFilterConfiguration Config => Plugin.Config.Filters.LevemeteFilter;

    private Dictionary<uint, string>? _levemetes { get; set; }

    public override void Reset()
    {
        Config.SelectedLevemete = 0;
    }

    public override bool HasValue()
    {
        return Config.SelectedLevemete != 0;
    }

    public override void Set(dynamic value)
    {
        Config.SelectedLevemete = (uint)value;
        Plugin.Config.Save();
    }

    public override void Draw()
    {
        if (_levemetes == null)
            return;

        using var id = ImRaii.PushId("LevemeteFilter");

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(t("LevemeteFilter.Label"));

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(InputWidth);
        using var combo = ImRaii.Combo("##Combo", _levemetes.TryGetValue(Config.SelectedLevemete, out var value) ? value : "All");
        if (!combo.Success)
            return;

        if (ImGui.Selectable(t("LevemeteFilter.Selectable.All") + "##All", Config.SelectedLevemete == 0))
        {
            Set(0);
            manager.Update();
        }

        if (Config.SelectedLevemete == 0)
        {
            ImGui.SetItemDefaultFocus();
        }

        foreach (var kv in _levemetes)
        {
            if (ImGui.Selectable($"{kv.Value}##Entry_{kv.Key}", Config.SelectedLevemete == kv.Key))
            {
                Set(kv.Key);
                manager.Update();
            }

            if (Config.SelectedLevemete == kv.Key)
            {
                ImGui.SetItemDefaultFocus();
            }
        }
    }

    public override bool Run()
    {
        _levemetes = state.Leves
            .Select(row => row.LevelLevemete.Value?.Object)
            .Where(item => item != null)
            .Cast<uint>()
            .GroupBy(item => item)
            .Select(group => GetRow<ENpcResident>(group.First()))
            .Where(item => item != null)
            .Cast<ENpcResident>()
            .Select(item => (item.RowId, Name: item.Singular.ToDalamudString().ToString()))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLevemete == 0)
            return false;

        var selection = state.Leves.Where(item => item.LevelLevemete.Value?.Object == Config.SelectedLevemete);
        if (!selection.Any())
        {
            Config.SelectedLevemete = 0;
            return false;
        }

        state.Leves = selection;

        return true;
    }
}
