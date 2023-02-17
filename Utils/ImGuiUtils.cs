using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using ImGuiScene;
using Lumina.Data.Files;

namespace LeveHelper;

public static class ImGuiUtils
{
    public static Vector4 ColorTransparent = new(0f, 0f, 0f, 0f);

    public static Vector4 ColorWhite = new(1f, 1f, 1f, 1f);
    public static Vector4 ColorGrey = new(0.5f, 0.5f, 0.5f, 1f);
    public static Vector4 ColorRed = new(1f, 0f, 0f, 1f);
    public static Vector4 ColorFreesia = new(246 / 255f, 195 / 255f, 36 / 255f, 1f); // #F6C324
    public static Vector4 ColorYellow = new(1f, 1f, 0f, 1f);
    public static Vector4 ColorYellowGreen = new(154 / 255f, 205 / 255f, 50 / 255f, 1f); // #9ACD32
    public static Vector4 ColorGreen = new(0f, 1f, 0f, 1f);

    public static Vector4 ColorMaelstorm = new(129f / 255f, 19f / 255f, 1f / 255f, 1f); // #811301
    public static Vector4 ColorAdder = new(165f / 255f, 115f / 255f, 0f, 1f); // #a57300
    public static Vector4 ColorLegion = new(72f / 255f, 89f / 255f, 55f / 255f, 1f); // #485937

    public static Vector4 ColorGroup = new(216f / 255f, 187f / 255f, 125f / 255f, 1f); // #D8BB7D

    private static readonly Dictionary<uint, TextureWrap> icons = new();

    public static void DrawIcon(uint iconId, int width = -1, int height = -1)
    {
        if (!icons.ContainsKey(iconId))
        {
            var tex = Service.Data.GetFile<TexFile>($"ui/icon/{iconId / 1000:D3}000/{iconId:D6}_hr1.tex");
            if (tex == null)
                return;

            var texWrap = Service.PluginInterface.UiBuilder.LoadImageRaw(tex.GetRgbaImageData(), tex.Header.Width, tex.Header.Height, 4);
            if (texWrap.ImGuiHandle == IntPtr.Zero)
                return;

            icons[iconId] = texWrap;
        }

        ImGui.Image(icons[iconId].ImGuiHandle, new(width == -1 ? icons[iconId].Width : width, height == -1 ? icons[iconId].Height : height));
    }

    public static void SameLineNoSpace()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, default(Vector2));
        ImGui.SameLine();
        ImGui.PopStyleVar();
    }

    public static void DrawFontAwesomeIcon(FontAwesomeIcon icon, Vector4 color)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextColored(color, icon.ToIconString());
        ImGui.PopFont();
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

            if (ingredient is CachedItem ingredientItem)
            {
                // filter crystals completely if we have enough
                if (ingredientItem.IsCrystal && ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                DrawItem(ingredientItem, ingredientAmount, $"{key}_{ingredient.ItemId}");

                // filter ingredients if we have enough
                if (ingredient.QuantityOwned >= ingredientAmount)
                    continue;

                if (ingredientItem.Ingredients.Any())
                {
                    var ingredientCount = (uint)(ingredientAmount / (double)(ingredientItem.Recipe?.AmountResult ?? 1));
                    DrawIngredients($"{key}_{ingredient.ItemId}", ingredientItem.Ingredients, ingredientCount, depth + 1);
                }
            }
        }

        if (depth > 0)
            ImGui.Unindent();
    }

    public static void DrawItem(CachedItem item, uint neededCount, string key = "Item", bool showIndicators = false, CachedTerritoryType? territoryType = null)
    {
        DrawIcon(item.Icon, 20, 20);
        ImGui.SameLine();

        // draw icons to the right: Gather, Vendor..
        var isLeveRequiredItem = Plugin.PluginWindow.LeveRequiredItems.Any(entry => entry.Item.ItemId == item.ItemId);

        var color = ColorWhite;

        if (item.QuantityOwned >= neededCount)
            color = ColorGreen;
        else if (item.QuantityOwned < neededCount || item.HasAllIngredients == false)
            color = ColorGrey;

        ImGui.PushStyleColor(ImGuiCol.Text, color);
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
                    ImGui.GetColorU32(ColorGrey),
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(ColorGrey, $"https://www.garlandtools.org/db/#item/{item.ItemId}");
                ImGui.EndTooltip();
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (item.IsCraftable == true)
            {
                unsafe
                {
                    var agent = (AgentRecipeNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.RecipeNote);
                    agent->OpenRecipeByItemId(item.ItemId);
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
                        var point = item.GatheringPoints.First(point => point.TerritoryTypeId == territoryType.RowId);
                        Service.GameFunctions.OpenMapWithGatheringPoint((Lumina.Excel.GeneratedSheets.GatheringPoint?)point.GatheringPoint, (CachedItem)item);
                    }
                    else
                    {
                        var agent = (AgentGatheringNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.GatheringNote);
                        agent->OpenGatherableByItemId((ushort)item.ItemId);
                    }

                    ImGui.SetWindowFocus(null);
                }
            }
            else if (item.IsFish == true)
            {
                Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First().FishingSpot, item);
                ImGui.SetWindowFocus(null);
            }
            else
            {
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.ItemId}"));
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
                        var agent = (AgentRecipeNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.RecipeNote);
                        agent->OpenRecipeByItemId(item.ItemId);
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
                        var point = item.GatheringPoints.First(point => point.TerritoryTypeId == territoryType.RowId);
                        Service.GameFunctions.OpenMapWithGatheringPoint((Lumina.Excel.GeneratedSheets.GatheringPoint?)point.GatheringPoint, (CachedItem)item);
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
                        var agent = (AgentGatheringNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.GatheringNote);
                        agent->OpenGatherableByItemId((ushort)item.ItemId);
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
                        Service.GameFunctions.OpenMapWithFishingSpot(item.FishingSpots.First().FishingSpot, item);
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
                        var agent = (nint)AgentModule.Instance()->GetAgentByInternalId(AgentId.FishGuide);
                        Service.GameFunctions.AgentFishGuide_OpenForItemId(agent, item.ItemId, item.IsSpearfishing);
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
                Service.GameFunctions.SearchForItem(item.ItemId);
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
                Task.Run(() => Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.ItemId}"));
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                ImGui.BeginTooltip();

                var pos = ImGui.GetCursorPos();
                ImGui.GetWindowDrawList().AddText(
                    UiBuilder.IconFont, 12,
                    ImGui.GetWindowPos() + pos + new Vector2(2),
                    ImGui.GetColorU32(ColorGrey),
                    FontAwesomeIcon.ExternalLinkAlt.ToIconString()
                );
                ImGui.SetCursorPos(pos + new Vector2(20, 0));
                ImGui.TextColored(ColorGrey, $"https://www.garlandtools.org/db/#item/{item.ItemId}");
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
            DrawIcon((uint)item.ClassJobIcon, 20, 20);
        }
    }
}
