using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HaselCommon.Extensions.Strings;
using HaselCommon.Graphics;
using HaselCommon.Gui;
using HaselCommon.Gui.ImGuiTable;
using HaselCommon.Services;
using HaselCommon.Services.SeStringEvaluation;
using ImGuiNET;
using LeveHelper.Config;
using Lumina.Excel.Sheets;

namespace LeveHelper.Tables.LeveListTableColumns;

[RegisterTransient]
public class NameColumn : ColumnString<Leve>
{
    private readonly PluginConfig _config;
    private readonly LeveService _leveService;
    private readonly ITextureProvider _textureProvider;
    private readonly SeStringEvaluatorService _seStringEvaluator;
    private readonly ImGuiContextMenuService _imGuiContextMenu;

    public NameColumn(
        PluginConfig config,
        LeveService leveService,
        ITextureProvider textureProvider,
        SeStringEvaluatorService seStringEvaluator,
        ImGuiContextMenuService imGuiContextMenu)
    {
        _config = config;
        _leveService = leveService;
        _textureProvider = textureProvider;
        _seStringEvaluator = seStringEvaluator;
        _imGuiContextMenu = imGuiContextMenu;

        LabelKey = "ListTab.Column.Name";
        Width = 2;

        if (!string.IsNullOrEmpty(_config.Filters.Name))
        {
            FilterValue = _config.Filters.Name;

            try
            {
                FilterRegex = new Regex(FilterValue, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            catch
            {
                FilterRegex = null;
            }
        }
    }

    public override string ToName(Leve row)
        => row.Name.ExtractText().StripSoftHypen();

    public override unsafe void DrawColumn(Leve row)
    {
        var isComplete = QuestManager.Instance()->IsLevequestComplete((ushort)row.RowId);

        if (ImGui.Selectable(ToName(row)))
        {
            if (_leveService.IsComplete(row) || _leveService.IsAccepted(row))
            {
                AgentQuestJournal.Instance()->OpenForQuest(row.RowId, 2);
            }
        }

        _imGuiContextMenu.Draw($"Leve{row.RowId}ContextMenu", builder =>
        {
            // TODO: builder.AddCopyName(textService, ToName(row));
            // TODO: Open in Journal
            builder.AddOpenOnGarlandTools("leve", row.RowId);
        });

        if (ImGui.IsItemHovered())
        {
            DrawLeveTooltip(row);
        }
    }

    private void DrawLeveTooltip(Leve leve)
    {
        using var id = ImRaii.PushId($"LeveTooltip{leve.RowId}");

        using var tooltip = ImRaii.Tooltip();
        if (!tooltip) return;

        var tooltipPos = ImGui.GetCursorScreenPos();

        IDalamudTextureWrap? icon = null;
        var hasIcon = false;

        if (leve.LeveVfx.IsValid && _textureProvider.TryGetFromGameIcon(leve.LeveVfx.Value.Icon, out var tex))
        {
            hasIcon = tex.TryGetWrap(out icon, out _);
        }

        using var popuptable = ImRaii.Table("PopupTable", hasIcon ? 2 : 1, ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.NoKeepColumnsVisible);
        if (!popuptable) return;

        var itemInnerSpacing = ImGui.GetStyle().ItemInnerSpacing * ImGuiHelpers.GlobalScale;
        var title = leve.Name.ExtractText();

        ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthFixed, Math.Max(ImGui.CalcTextSize(title).X + itemInnerSpacing.X, 300 * ImGuiHelpers.GlobalScale));

        if (hasIcon && icon != null)
            ImGui.TableSetupColumn("Icon", ImGuiTableColumnFlags.WidthFixed, icon.Width / 2f * ImGuiHelpers.GlobalScale);

        ImGui.TableNextColumn(); // Text
        using var indentSpacing = ImRaii.PushStyle(ImGuiStyleVar.IndentSpacing, itemInnerSpacing.X); // TODO: maybe changing cellpadding would be better
        using var indent = ImRaii.PushIndent(1);

        ImGui.TextUnformatted(title);

        var subTitleBuilder = new StringBuilder();

        subTitleBuilder.Append(_seStringEvaluator.EvaluateFromAddon(35, new SeStringContext() { LocalParameters = [(uint)leve.ClassJobLevel] }));

        if (leve.JournalGenre.IsValid)
            subTitleBuilder.Append(" • " + leve.JournalGenre.Value.Name.ExtractText());

        if (subTitleBuilder.Length > 0)
        {
            ImGuiUtils.PushCursorY(-3 * ImGuiHelpers.GlobalScale);
            using (ImRaii.PushColor(ImGuiCol.Text, (uint)Color.Grey))
                ImGui.TextUnformatted(subTitleBuilder.ToString());
        }

        if (leve.IconIssuer != 0 && _textureProvider.TryGetFromGameIcon(leve.IconIssuer, out var imageTex) && imageTex.TryGetWrap(out var image, out _))
        {
            DrawSeparator(marginTop: 1, marginBottom: 5);
            ImGui.Image(image.ImGuiHandle, GetContainedSize(image.Size));
        }

        DrawSeparator(marginTop: 1, marginBottom: 4);
        ImGuiHelpers.SeStringWrapped(_seStringEvaluator.Evaluate(leve.Description));

        if (hasIcon && icon != null)
        {
            ImGui.TableNextColumn(); // Icon
            ImGui.Image(icon.ImGuiHandle, icon.Size / 2f);
        }
    }

    private static Vector2 GetContainedSize(Vector2 imageSize)
    {
        var newWidth = ImGui.GetContentRegionAvail().X;
        var ratio = newWidth / imageSize.X;
        var newHeight = imageSize.Y * ratio;
        return new Vector2(newWidth, newHeight);
    }

    private static void DrawSeparator(float marginTop = 2, float marginBottom = 5)
    {
        ImGuiUtils.PushCursorY(marginTop * ImGuiHelpers.GlobalScale);
        var pos = ImGui.GetCursorScreenPos();
        ImGui.GetWindowDrawList().AddLine(pos, pos + new Vector2(ImGui.GetContentRegionAvail().X, 0), ImGui.GetColorU32(ImGuiCol.Separator));
        ImGuiUtils.PushCursorY(marginBottom * ImGuiHelpers.GlobalScale);
    }
}
