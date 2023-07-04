using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using LeveHelper.Sheets;

namespace LeveHelper.Utils;

public static partial class ImGuiUtils
{
    public static void DrawFontAwesomeIcon(FontAwesomeIcon icon, Vector4 color)
    {
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            ImGui.TextColored(color, icon.ToIconString());
        }
        ImGui.SameLine();
    }

    public static void DrawIngredients(string key, RequiredItem[] ingredients, uint parentAmount = 1, int depth = 0)
    {
        if (depth > 0)
            ImGui.Indent();

        foreach (var entry in ingredients)
        {
            var ingredient = entry.Item;
            var ingredientAmount = entry.Amount * parentAmount;

            if (ingredient is Item ingredientItem)
            {
                // filter crystals completely if we have enough
                if (ingredientItem.IsCrystal && ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                DrawItem(ingredientItem, ingredientAmount, $"{key}_{ingredient.RowId}");

                // filter ingredients if we have enough
                if (ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                if (ingredientItem.Ingredients.Any())
                {
                    var ingredientCount = (uint)(ingredientAmount / (double)(ingredientItem.Recipe?.AmountResult ?? 1));
                    DrawIngredients($"{key}_{ingredient.RowId}", ingredientItem.Ingredients, ingredientCount, depth + 1);
                }
            }
        }

        if (depth > 0)
            ImGui.Unindent();
    }

    public static void DrawItem(Item item, uint neededCount, string key = "Item", bool showIndicators = false, TerritoryType? territoryType = null)
    {
        Plugin.PluginWindow.TextureManager.GetIcon(item.Icon).Draw(new(20));
        ImGui.SameLine();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = Plugin.PluginWindow.LeveRequiredItems.Any(entry => entry.Item.RowId == item.RowId);

        var color = Colors.White;

        if (item.QuantityOwned >= neededCount)
            color = Colors.Green;
        else if (item.QuantityOwned < neededCount || item.HasAllIngredients == false)
            color = Colors.Grey;

        ImGui.PushStyleColor(ImGuiCol.Text, (uint)color);
        ImGui.Selectable($"{item.QuantityOwned}/{neededCount} {item.Name}{(isLeveRequiredItem ? (char)SeIconChar.HighQuality : "")}##{key}_Selectable");
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (item.IsCraftable == true)
            {
                ImGui.SetTooltip(StringUtil.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (item.IsGatherable == true)
            {
                if (territoryType != null)
                {
                    ImGui.SetTooltip(StringUtil.GetAddonText(8506)); // "Open Map"
                }
                else
                {
                    ImGui.SetTooltip(StringUtil.GetAddonText(1472)); // "Search for Item by Gathering Method"
                }
            }
            else if (item.IsFish == true)
            {
                ImGui.SetTooltip(StringUtil.GetAddonText(8506)); // "Open Map"
            }
            else
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();
                ImGui.Text("Open on GarlandTools");

                var pos = ImGui.GetCursorPos();
                ImGui.GetWindowDrawList().AddText(
                    UiBuilder.IconFont, 12,
                    ImGui.GetWindowPos() + pos + new Vector2(2),
                    Colors.Grey,
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(Colors.Grey, $"https://www.garlandtools.org/db/#item/{item.RowId}");
                ImGui.EndTooltip();
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (item.IsCraftable == true)
            {
                unsafe
                {
                    AgentRecipeNote.Instance()->OpenRecipeByItemId(item.RowId);
                    ImGui.SetWindowFocus(null);
                }
            }
            // TODO: preferance setting?
            else if (item.IsGatherable == true)
            {
                unsafe
                {
                    if (territoryType != null)
                    {
                        var point = item.GatheringPoints.First(point => point.TerritoryType.Row == territoryType.RowId);
                        Service.GameFunctions.OpenMapWithGatheringPoint(point, item);
                    }
                    else
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                    }

                    ImGui.SetWindowFocus(null);
                }
            }
            else if (item.IsFish == true)
            {
                Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First(), item);
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
        }

        if (ImGui.BeginPopupContextItem($"##ItemContextMenu_{key}_Tooltip"))
        {
            var showSeparator = false;

            if (item.IsCraftable == true)
            {
                if (ImGui.Selectable(StringUtil.GetAddonText(1414))) // "Search for Item by Crafting Method"
                {
                    unsafe
                    {
                        AgentRecipeNote.Instance()->OpenRecipeByItemId(item.RowId);
                        ImGui.SetWindowFocus(null);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                showSeparator = true;
            }

            if (item.IsGatherable == true)
            {
                if (territoryType != null)
                {
                    if (ImGui.Selectable(StringUtil.GetAddonText(8506))) // "Open Map"
                    {
                        var point = item.GatheringPoints.First(point => point.TerritoryType.Row == territoryType.RowId);
                        Service.GameFunctions.OpenMapWithGatheringPoint(point, item);
                        ImGui.SetWindowFocus(null);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }
                }

                if (ImGui.Selectable(StringUtil.GetAddonText(1472))) // "Search for Item by Gathering Method"
                {
                    unsafe
                    {
                        AgentGatheringNote.Instance()->OpenGatherableByItemId((ushort)item.RowId);
                        ImGui.SetWindowFocus(null);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                showSeparator = true;
            }

            if (item.IsFish == true)
            {
                if (territoryType != null)
                {
                    if (ImGui.Selectable(StringUtil.GetAddonText(8506))) // "Open Map"
                    {
                        Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First(), item);
                        ImGui.SetWindowFocus(null);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }
                }

                if (ImGui.Selectable("Open in Fish Guide"))
                {
                    unsafe
                    {
                        var agent = (AgentFishGuide*)AgentModule.Instance()->GetAgentByInternalId(AgentId.FishGuide);
                        agent->OpenForItemId(item.RowId, item.IsSpearfishing);
                        ImGui.SetWindowFocus(null);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                showSeparator = true;
            }

            if (showSeparator)
                ImGui.Separator();

            if (ImGui.Selectable(StringUtil.GetAddonText(4379))) // "Search for Item"
            {
                unsafe
                {
                    ItemFinderModule.Instance()->SearchForItem(item.RowId);
                }
                ImGui.SetWindowFocus(null);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }

            if (ImGui.Selectable(StringUtil.GetAddonText(159))) // "Copy Item Name"
            {
                ImGui.SetClipboardText(item.Name);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }

            if (ImGui.Selectable("Open on GarlandTools"))
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.RowId}"));
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();

                var pos = ImGui.GetCursorPos();
                ImGui.GetWindowDrawList().AddText(
                    UiBuilder.IconFont, 12,
                    ImGui.GetWindowPos() + pos + new Vector2(2),
                    Colors.Grey,
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(Colors.Grey, $"https://www.garlandtools.org/db/#item/{item.RowId}");
                ImGui.EndTooltip();
            }

            // TODO: search on market

            ImGui.EndPopup();
        }

        if (showIndicators && item.ClassJobIcon != null)
        {
            var pos = ImGui.GetCursorPos();
            var availSize = ImGui.GetContentRegionAvail();
            ImGui.SameLine(availSize.X - pos.X, 0); // TODO: no -20 here??
            Plugin.PluginWindow.TextureManager.GetIcon((int)item.ClassJobIcon).Draw(new(20));
        }
    }
}
