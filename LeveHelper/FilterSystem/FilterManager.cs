using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility.Raii;
using HaselCommon.Graphics;
using HaselCommon.Services;
using ImGuiNET;
using LeveHelper.Caches;
using LeveHelper.Config;
using LeveHelper.Interfaces;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public class FilterManager : IDisposable
{
    private readonly PluginConfig PluginConfig;
    private readonly TextService TextService;
    private readonly LeveIssuerCache LeveIssuerCache;
    private readonly LeveService LeveService;

    public FiltersState State { get; set; }
    public List<IFilter> Filters { get; private set; } = [];

    public FilterManager(
        FiltersState filtersState,
        PluginConfig pluginConfig,
        TextService textService,
        LeveIssuerCache leveIssuerCache,
        LeveService leveService,
        IEnumerable<IFilter> filters)
    {
        State = filtersState;
        PluginConfig = pluginConfig;
        TextService = textService;
        LeveIssuerCache = leveIssuerCache;
        LeveService = leveService;

        foreach (var filter in filters)
        {
            filter.FilterManager = this;
            Filters.Add(filter);
        }

        Filters.Sort((filter1, filter2) => filter2.Order - filter1.Order);

        Update();

        TextService.LanguageChanged += OnLanguageChanged;
    }

    public void Dispose()
    {
        TextService.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(string langCode)
    {
        State.Reload();
        Update();
    }

    private uint byId(Leve item)
        => item.RowId;

    private ushort byLevel(Leve item)
        => item.ClassJobLevel;

    private string byType(Leve item)
        => item.LeveAssignmentType.Value?.Name ?? "";

    private string byName(Leve item)
        => item.Name.AsReadOnly().ExtractText();

    private string byIssuer(Leve item)
    {
        if (!LeveIssuerCache.TryGetValue(item.RowId, out var issuers))
            return string.Empty;

        return TextService.GetENpcResidentName(issuers.First().RowId);
    }

    public void Update()
    {
        State.Leves = State.AllLeves;

        foreach (var filter in Filters)
        {
            if (filter.Run())
            {
                // state changed
            }
        }

        State.Leves = (State.SortColumnIndex, State.SortDirection) switch
        {
            (0, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(byId),
            (0, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(byId),

            (1, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(byLevel),
            (1, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(byLevel),

            (2, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(byType),
            (2, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(byType),

            (3, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(byName),
            (3, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(byName),

            (4, ImGuiSortDirection.Ascending) => State.Leves.OrderBy(byIssuer),
            (4, ImGuiSortDirection.Descending) => State.Leves.OrderByDescending(byIssuer),

            _ => State.Leves
        };

        State.LevesArray = State.Leves.ToArray();

        State.NumCompletedLeves = State.Leves.Where(LeveService.IsComplete).Count();
        State.NumTotalLeves = State.Leves.Count();
        State.NeededAllowances = State.Leves
            .Where(row => !LeveService.IsComplete(row) && !LeveService.IsAccepted(row))
            .Select(item => item.AllowanceCost)
            .Aggregate(0, (total, cost) => total + cost);
    }

    public void Reload()
    {
        State.Reload();

        foreach (var filter in Filters)
        {
            filter.Reload();
        }

        Update();
    }

    public void Reset()
    {
        foreach (var filter in Filters)
        {
            filter.Reset();
        }

        PluginConfig.Save();
        Update();
    }

    public T GetFilter<T>() where T : IFilter
    {
        return (T)Filters.Single(item => item.GetType() == typeof(T));
    }

    public void Draw()
    {
        using var id = ImRaii.PushId("Filters");

        var someFilterSet = Filters.Any(filter => filter.HasValue());

        using var treeNodeColor = someFilterSet ? ImRaii.PushColor(ImGuiCol.Text, (uint)Color.Green) : null;
        using var treeNode = ImRaii.TreeNode(TextService.Translate("FilterManager.Title"), ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.FramePadding);
        if (!treeNode.Success)
            return;
        treeNodeColor?.Dispose();

        using var table = ImRaii.Table("Filters", 2, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.NoSavedSettings, new(-1, 100));
        if (!table.Success)
            return;

        ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed, 100);
        ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.WidthStretch);

        foreach (var filter in Filters)
        {
            ImGui.TableNextRow();
            filter.Draw();
        }

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();

        if (ImGui.Button(TextService.Translate("FilterManager.ClearFilters")))
        {
            Reset();
        }
    }
}
