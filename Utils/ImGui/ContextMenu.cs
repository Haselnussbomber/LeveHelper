using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using LeveHelper.Records;
using LeveHelper.Sheets;

namespace LeveHelper.Utils;

public partial class ImGuiUtils
{
    public class ContextMenu : List<IContextMenuEntry>
    {
        private readonly string key;

        public ContextMenu(string key)
        {
            this.key = key;
        }

        public void Draw()
        {
            using var popup = ImRaii.ContextPopupItem(key);
            if (!popup.Success)
                return;

            var visibleEntries = this.Where(entry => entry.Visible);
            var count = visibleEntries.Count();
            var i = 0;
            foreach (var entry in visibleEntries)
            {
                entry.Draw(new IterationArgs(i++, count));
            }
        }
    }

    public interface IContextMenuEntry
    {
        public bool Visible { get; set; }
        public bool Enabled { get; set; }
        public string Label { get; set; }
        public bool LoseFocusOnClick { get; set; }
        public Action? ClickCallback { get; set; }
        public Action? HoverCallback { get; set; }
        public void Draw(IterationArgs args);
    }

    public record ContextMenuSeparator : IContextMenuEntry
    {
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public bool LoseFocusOnClick { get; set; } = false;
        public string Label { get; set; } = string.Empty;
        public Action? ClickCallback { get; set; } = null;
        public Action? HoverCallback { get; set; } = null;

        public void Draw(IterationArgs args)
        {
            if (!args.IsFirst && !args.IsLast)
                ImGui.Separator();
        }
    }

    public record ContextMenuEntry : IContextMenuEntry
    {
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public bool LoseFocusOnClick { get; set; } = false;
        public string Label { get; set; } = string.Empty;
        public Action? ClickCallback { get; set; } = null;
        public Action? HoverCallback { get; set; } = null;

        public void Draw(IterationArgs args)
        {
            if (ImGui.MenuItem(Label, Enabled))
            {
                ClickCallback?.Invoke();

                if (LoseFocusOnClick)
                {
                    ImGui.SetWindowFocus(null);
                }
            }
            if (ImGui.IsItemHovered())
            {
                HoverCallback?.Invoke();
            }
        }

        public static unsafe ContextMenuEntry CreateTryOn(uint ItemId, uint GlamourItemId = 0, byte StainId = 0)
            => new()
            {
                Visible = ItemUtils.CanTryOn(ItemId),
                Label = GetAddonText(2426), // "Try On"
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || ImGui.IsKeyDown(ImGuiKey.RightShift))
                        AgentTryon.TryOn(0, ItemId, StainId, 0, 0);
                    else
                        AgentTryon.TryOn(0, ItemId, StainId, GlamourItemId, StainId);
                }
            };

        public static unsafe ContextMenuEntry CreateSearchCraftingMethod(Item item)
            => new()
            {
                Visible = item.IsCraftable,
                Label = GetAddonText(1414), // "Search for Item by Crafting Method"
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    AgentRecipeNote.Instance()->OpenRecipeByItemId(item.RowId);
                }
            };

        public static unsafe ContextMenuEntry CreateOpenMapForGatheringPoint(Item item, Lumina.Excel.GeneratedSheets.TerritoryType? territoryType)
            => new()
            {
                Visible = item.IsGatherable && territoryType != null,
                Label = GetAddonText(8506), // "Open Map"
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    var point = item.GatheringPoints.First(point => point.TerritoryType.Row == territoryType!.RowId);
                    Service.GameFunctions.OpenMapWithGatheringPoint(point, item);
                }
            };

        public static unsafe ContextMenuEntry CreateOpenMapForFishingSpot(Item item, Lumina.Excel.GeneratedSheets.TerritoryType? territoryType)
            => new()
            {
                Visible = item.IsFish && territoryType != null,
                Label = GetAddonText(8506), // "Open Map"
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First(), item);
                }
            };

        public static unsafe ContextMenuEntry CreateSearchGatheringMethod(Item item)
            => new()
            {
                Visible = item.IsGatherable,
                Label = GetAddonText(1472), // "Search for Item by Gathering Method"
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                }
            };

        public static unsafe ContextMenuEntry CreateOpenInFishGuide(Item item)
            => new()
            {
                Visible = item.IsFish,
                Label = t("ItemContextMenu.OpenInFishGuide"),
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    var agent = (AgentFishGuide*)AgentModule.Instance()->GetAgentByInternalId(AgentId.FishGuide);
                    agent->OpenForItemId(item.RowId, item.IsSpearfishing);
                }
            };

        public static unsafe ContextMenuEntry CreateItemFinder(uint ItemId)
            => new()
            {
                Label = GetAddonText(4379), // "Search for Item"
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    ItemFinderModule.Instance()->SearchForItem(ItemId);
                }
            };

        public static ContextMenuEntry CreateOpenOnGarlandTools(uint ItemId)
            => new()
            {
                Label = t("ItemContextMenu.OpenOnGarlandTools"),
                ClickCallback = () =>
                {
                    Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{ItemId}"));
                },
                HoverCallback = () =>
                {
                    using var tooltip = ImRaii.Tooltip();

                    var pos = ImGui.GetCursorPos();
                    ImGui.GetWindowDrawList().AddText(
                        UiBuilder.IconFont, 12 * ImGuiHelpers.GlobalScale,
                        ImGui.GetWindowPos() + pos + new Vector2(2),
                        Colors.Grey,
                        FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                    );
                    ImGui.SetCursorPos(pos + new Vector2(20, 0) * ImGuiHelpers.GlobalScale);
                    TextUnformattedColored(Colors.Grey, $"https://www.garlandtools.org/db/#item/{ItemId}");
                }
            };

        public static ContextMenuEntry CreateCopyItemName(uint ItemId)
            => new()
            {
                Label = GetAddonText(159), // "Copy Item Name"
                ClickCallback = () =>
                {
                    var itemName = GetRow<Item>(ItemId % 1000000)?.Singular.RawString ?? string.Empty;
                    ImGui.SetClipboardText(itemName);
                }
            };

        public static ContextMenuEntry CreateItemSearch(uint ItemId)
            => new()
            {
                Visible = ItemSearchUtils.CanSearchForItem(ItemId),
                Label = t("ItemContextMenu.SearchTheMarkets"),
                LoseFocusOnClick = true,
                ClickCallback = () =>
                {
                    ItemSearchUtils.Search(ItemId);
                }
            };

        public static ContextMenuSeparator CreateSeparator() => new();
    }
}
