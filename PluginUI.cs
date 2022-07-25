using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using ImGuiNET;
using LeveHelper.Filters;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public unsafe partial class PluginUi
{
    private Plugin Plugin { get; init; }

    internal uint PlaceNameId;

    private FilterManager filterManager;

    //private byte PlayerCityState => *(byte*)((IntPtr)UIState.Instance() + 0xA38 + 0x13A); // UIState.PlayerState.CityState

    private bool _show = false;

    internal bool Show
    {
        get => _show;
        set => _show = value;
    }

    public PluginUi(Plugin plugin)
    {
        Plugin = plugin;

        Service.PluginInterface.UiBuilder.Draw += Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        UpdatePlaceName();

        Service.Commands.AddHandler("/levehelper", new(delegate { Show = !Show; })
        {
            HelpMessage = "Show Window"
        });

        Service.Commands.AddHandler("/lh", new(delegate { Show = !Show; })
        {
            HelpMessage = "Show Window"
        });

        filterManager = new(plugin, this);

#if DEBUG
        _show = true;
#endif
    }

    private void ClientState_TerritoryChanged(object? sender, ushort e)
    {
        UpdatePlaceName();
    }

    private void UpdatePlaceName()
    {
        var curTerritory = Service.Data.GetExcelSheet<TerritoryType>()?.GetRow(Service.ClientState.TerritoryType);
        PlaceNameId = curTerritory?.PlaceName?.Row ?? 0;
    }

    private void OpenConfig()
    {
        Show = true;
    }

    private void Draw()
    {
#if DEBUG
        ImGui.SetNextWindowSize(new Vector2(830f, 600f), ImGuiCond.Appearing);
#else
        ImGui.SetNextWindowSize(new Vector2(830f, 600f), ImGuiCond.FirstUseEver);
#endif

        if (!Show || !Service.ClientState.IsLoggedIn)
        {
            return;
        }

        if (!ImGui.Begin(Plugin.Name, ref _show))
        {
            ImGui.End();
            return;
        }

        filterManager.Draw();

        ImGui.Separator();

        var questManager = QuestManagerHelper.Instance;
        var state = filterManager.state;

        ImGui.Text($"Accepted Leves: {questManager.NumActiveLevequests}/16");
        ImGui.SameLine();
        ImGui.Text($"Allowances: {questManager.NumAllowances}/100 (need {state.neededAllowances} over {Math.Ceiling(state.numTotalLeves / 6f)} days, next in {questManager.NextAllowances - DateTime.Now:hh':'mm':'ss})");
        ImGui.SameLine();

        var percent = (state.numCompletedLeves / (float)state.numTotalLeves * 100f).ToString("0.00", CultureInfo.InvariantCulture);
        ImGui.Text($"Completion: {state.numCompletedLeves}/{state.numTotalLeves} ({percent}%%)");

        ImGui.Separator();

        if (!ImGui.BeginChild("LeveHelper_TableWrapper", new Vector2(-1), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.EndChild();
            ImGui.End();
            return;
        }

        if (!ImGui.BeginTable("LeveHelper_Table", 6, ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable | ImGuiTableFlags.NoSavedSettings, ImGui.GetContentRegionAvail()))
        {
            ImGui.EndTable();
            ImGui.EndChild();
            ImGui.End();
            return;
        }

        var specs = ImGui.TableGetSortSpecs();
        if (specs.NativePtr != null && specs.SpecsDirty)
        {
            state.sortColumnIndex = specs.Specs.ColumnIndex;
            state.sortDirection = specs.Specs.SortDirection;
            specs.SpecsDirty = false;
            filterManager.Update();
        }

        ImGui.TableSetupColumn("Id");
        ImGui.TableSetupColumn("Level");
        ImGui.TableSetupColumn("Name");
        ImGui.TableSetupColumn("Class");
        ImGui.TableSetupColumn("Levemete");
        ImGui.TableSetupColumn("Allowance Cost");
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();

        foreach (LeveRecord item in state.levesArray)
        {
            ImGui.TableNextRow();

            // Id
            ImGui.TableNextColumn();
            ImGui.Text(item.RowId);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Left Click: Open on GarlandTools");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                Dalamud.Utility.Util.OpenLink($"https://www.garlandtools.org/db/#leve/{item.RowId}");
            }

            // Level
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassJobLevel);

            // Name
            ImGui.TableNextColumn();
            if (item.TownLocked)
            {
                ImGui.Text("*");
                ImGui.SameLine();
            }

            ImGui.TextColored(item.IsComplete ? ImGuiUtils.ColorGreen : ImGuiUtils.ColorRed, item.Name);

            if (item.TownLocked)
            {
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text($"Only available to Characters that started in {item.TownName}.");
                    ImGui.EndTooltip();
                }
            }

            // Class
            ImGui.TableNextColumn();
            ImGui.Text(item.ClassName);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Right Click: Filter by Class");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                filterManager.SetValue<ClassFilter>((uint)item.leve.Unknown4);
            }

            // Levemete
            ImGui.TableNextColumn();
            ImGui.Text(item.LevemeteName);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.SetTooltip("Left Click: Open Map\nRight Click: Filter by Levemate");
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                item.leve.LevelLevemete.Value?.OpenMapLocation();
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                filterManager.SetValue<LevemeteFilter>(item.leve.LevelLevemete.Value!.Object);
            }

            // AllowanceCost
            ImGui.TableNextColumn();
            ImGui.Text(item.leve.AllowanceCost.ToString());
        }

        ImGui.EndTable();
        ImGui.EndChild();
        ImGui.End();
    }
}

public sealed partial class PluginUi : IDisposable
{
    private bool isDisposed;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (isDisposed)
            return;

        if (disposing)
        {
            Show = false;

            Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;

            Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;
            Service.PluginInterface.UiBuilder.Draw -= Draw;

            Service.Commands.RemoveHandler("/levehelper");
            Service.Commands.RemoveHandler("/lh");
        }

        isDisposed = true;
    }
}
