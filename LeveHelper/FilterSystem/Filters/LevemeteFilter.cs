using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility.Raii;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Caches;
using LeveHelper.Config;
using LeveHelper.Interfaces;

namespace LeveHelper.Filters;

public class LevemeteFilterConfiguration
{
    public uint SelectedLevemete = 0;
}

[RegisterSingleton<IFilter>(Duplicate = DuplicateStrategy.Append)]
public class LevemeteFilter(
    PluginConfig PluginConfig,
    TextService TextService,
    LeveIssuerCache LeveIssuerCache)
    : IFilter
{
    public int Order => 4;
    public LevemeteFilterConfiguration Config => PluginConfig.Filters.LevemeteFilter;

    public FilterManager? FilterManager { get; set; }

    private Dictionary<uint, string>? Levemetes { get; set; }

    public void Reload()
    {
        Levemetes?.Clear();
        Run();
    }

    public void Reset()
    {
        Config.SelectedLevemete = 0;
    }

    public bool HasValue()
    {
        return Config.SelectedLevemete != 0;
    }

    public void Set(dynamic value)
    {
        Config.SelectedLevemete = (uint)value;
        PluginConfig.Save();
    }

    public void Draw()
    {
        if (Levemetes == null)
            return;

        using var id = ImRaii.PushId("LevemeteFilter");

        ImGui.TableNextColumn();
        TextService.Draw("LevemeteFilter.Label");

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(250);
        using var combo = ImRaii.Combo("##Combo", Levemetes.TryGetValue(Config.SelectedLevemete, out var value) ? value : TextService.Translate("LevemeteFilter.Selectable.All"));
        if (!combo.Success)
            return;

        if (ImGui.Selectable(TextService.Translate("LevemeteFilter.Selectable.All") + "##All", Config.SelectedLevemete == 0))
        {
            Set(0);
            FilterManager!.Update();
        }

        if (Config.SelectedLevemete == 0)
        {
            ImGui.SetItemDefaultFocus();
        }

        foreach (var kv in Levemetes)
        {
            if (ImGui.Selectable($"{kv.Value}##Entry_{kv.Key}", Config.SelectedLevemete == kv.Key))
            {
                Set(kv.Key);
                FilterManager!.Update();
            }

            if (Config.SelectedLevemete == kv.Key)
            {
                ImGui.SetItemDefaultFocus();
            }
        }
    }

    public bool Run()
    {
        Levemetes = FilterManager!.State.Leves
            .Select(row => LeveIssuerCache.GetValue(row.RowId) ?? [])
            .Where(issuers => issuers.Length != 0)
            .SelectMany(resident => resident)
            .Distinct()
            .Select(item => (item.RowId, Name: TextService.GetENpcResidentName(item.RowId)))
            .OrderBy(item => item.Name)
            .ToDictionary(item => item.RowId, item => item.Name);

        if (Config.SelectedLevemete == 0)
            return false;

        var selection = FilterManager.State.Leves.Where(item => (LeveIssuerCache.GetValue(item.RowId) ?? []).Select(issuer => issuer.RowId).Contains(Config.SelectedLevemete));
        if (!selection.Any())
        {
            Config.SelectedLevemete = 0;
            return false;
        }

        FilterManager.State.Leves = selection;

        return true;
    }
}
