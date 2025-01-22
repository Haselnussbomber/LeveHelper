using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Gui;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Config;
using LeveHelper.Interfaces;

namespace LeveHelper.Filters;

public class LevelFilterConfiguration
{
    public int MinLevel = 0;
    public int MaxLevel = 0;
}

[RegisterSingleton<IFilter>(Duplicate = DuplicateStrategy.Append)]
public class LevelFilter(PluginConfig PluginConfig, TextService TextService) : IFilter
{
    public int Order => 1;
    public LevelFilterConfiguration Config => PluginConfig.Filters.LevelFilter;

    public FilterManager? FilterManager { get; set; }

    public void Reload()
    {
    }

    public void Reset()
    {
        Config.MinLevel = 0;
        Config.MaxLevel = 0;
    }

    public bool HasValue()
    {
        return Config.MinLevel != 0 || Config.MaxLevel != 0;
    }

    public unsafe void Draw()
    {
        using var id = ImRaii.PushId("LevelFilter");

        var playerState = PlayerState.Instance();
        var maxLevel = playerState->IsLoaded == 1 ? playerState->MaxLevel : 100;

        void DrawInput(string key, ref int value)
        {
            ImGui.TableNextColumn();
            TextService.Draw($"LevelFilter.{key}.Label");

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(250);
            if (ImGui.SliderInt("###Input" + key, ref value, 0, maxLevel))
            {
                PluginConfig.Save();
                FilterManager!.Update();
            }
            ImGui.SameLine();
            if (ImGuiUtils.IconButton("Reset###Reset" + key, FontAwesomeIcon.Undo, TextService.GetAddonText(4830) ?? "Reset"))
            {
                value = 0;
                PluginConfig.Save();
                FilterManager!.Update();
            }
        }

        DrawInput("MinLevel", ref Config.MinLevel);
        DrawInput("MaxLevel", ref Config.MaxLevel);
    }

    public bool Run()
    {
        var modified = false;

        if (Config.MinLevel != 0)
        {
            FilterManager!.State.Leves = FilterManager.State.Leves.Where(leve => leve.ClassJobLevel >= Config.MinLevel);
            modified = true;
        }

        if (Config.MaxLevel != 0)
        {
            FilterManager!.State.Leves = FilterManager.State.Leves.Where(leve => leve.ClassJobLevel <= Config.MaxLevel);
            modified = true;
        }

        return modified;
    }
}
