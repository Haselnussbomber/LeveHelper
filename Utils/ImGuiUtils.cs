using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public static Vector4 ColorYellow = new(1f, 1f, 0f, 1f);
    public static Vector4 ColorGreen = new(0f, 1f, 0f, 1f);

    public static Vector4 ColorMaelstorm = new(129f / 255f, 19f / 255f, 1f / 255f, 1f); // #811301
    public static Vector4 ColorAdder = new(165f / 255f, 115f / 255f, 0f, 1f); // #a57300
    public static Vector4 ColorLegion = new(72f / 255f, 89f / 255f, 55f / 255f, 1f); // #485937

    public static Vector4 ColorGroup = new(216f / 255f, 187f / 255f, 125f / 255f, 1f); // #D8BB7D

    private static readonly Dictionary<int, TextureWrap> icons = new();

    public static void DrawIcon(int iconId, int width = -1, int height = -1)
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

    public static void DrawIngredients(string key, RequiredItem[] ingredients, uint parentCount = 1, int depth = 0)
    {
        if (depth > 0)
            ImGui.Indent();

        foreach (var entry in ingredients)
        {
            var ingredient = entry.Item;
            var ingredientCount = entry.Amount * parentCount;

            if (ingredient.IsCrystal && ingredient.QuantityOwned >= ingredientCount)
                continue;

            DrawItem(ingredient, ingredientCount, $"{key}_{ingredient.ItemId}");

            if (ingredient.QuantityOwned >= ingredientCount)
                continue;

            if (ingredient.Ingredients.Any())
            {
                DrawIngredients($"{key}_{ingredient.ItemId}", ingredient.Ingredients, entry.Amount, depth + 1);
            }
        }

        if (depth > 0)
            ImGui.Unindent();
    }

    public static void DrawItem(CachedItem item, uint neededCount, string key = "Item")
    {
        DrawIcon(item.Icon, 20, 20);
        ImGui.SameLine();

        // draw icons to the right: Gather, Vendor..

        var color = ColorWhite;

        if (item.QuantityOwned >= neededCount)
            color = ColorGreen;
        else if (item.QuantityOwned < neededCount || !item.HasAllIngredients)
            color = ColorGrey;

        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Selectable($"{item.QuantityOwned}/{neededCount} {item.ItemName}##{key}_Selectable");
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            // TODO: info about what leve/recipe needs this?

            if (item.IsCraftable)
            {
                ImGui.SetTooltip(StringUtil.GetAddonText(1414)); // "Search for Item by Crafting Method"
            }
            else if (item.IsGatherable)
            {
                ImGui.SetTooltip(StringUtil.GetAddonText(1472)); // "Search for Item by Gathering Method"
            }
            else if (item.FishingSpot != null)
            {
                ImGui.SetTooltip("Show Fishing Spot");
            }
            else
            {
                ImGui.SetTooltip("Open on GarlandTools");
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (item.IsCraftable)
            {
                unsafe
                {
                    var agent = (AgentRecipeNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.RecipeNote);
                    agent->OpenRecipeByItemId(item.ItemId);
                }
            }
            // TODO: preferance setting?
            else if (item.IsGatherable)
            {
                unsafe
                {
                    var agent = (AgentGatheringNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.GatheringNote);
                    agent->OpenGatherableByItemId((ushort)item.ItemId);
                }
            }
            else if (item.FishingSpot != null)
            {
                Service.GameFunctions.OpenMapWithGatheringPoint(item.FishingSpot);
            }
            else
            {
                Dalamud.Utility.Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.ItemId}");
            }
        }

        if (ImGui.BeginPopupContextItem($"ItemContextMenu##{key}_Tooltip"))
        {
            var showSeparator = false;

            if (item.IsCraftable)
            {
                if (ImGui.Selectable(StringUtil.GetAddonText(1414))) // "Search for Item by Crafting Method"
                {
                    unsafe
                    {
                        var agent = (AgentRecipeNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.RecipeNote);
                        agent->OpenRecipeByItemId(item.ItemId);
                    }
                }

                showSeparator = true;
            }

            if (item.IsGatherable)
            {
                if (ImGui.Selectable(StringUtil.GetAddonText(1472))) // "Search for Item by Gathering Method"
                {
                    unsafe
                    {
                        var agent = (AgentGatheringNote*)AgentModule.Instance()->GetAgentByInternalId(AgentId.GatheringNote);
                        agent->OpenGatherableByItemId((ushort)item.ItemId);
                    }
                }

                showSeparator = true;
            }

            if (item.FishingSpot != null)
            {
                if (ImGui.Selectable("Show Fishing Spot"))
                {
                    Service.GameFunctions.OpenMapWithGatheringPoint(item.FishingSpot);
                }

                showSeparator = true;
            }

            if (showSeparator)
                ImGui.Separator();

            if (ImGui.Selectable(StringUtil.GetAddonText(4379))) // "Search for Item"
            {
                Service.GameFunctions.SearchForItem(item.ItemId);
            }

            if (ImGui.Selectable(StringUtil.GetAddonText(159))) // "Copy Item Name"
            {
                ImGui.SetClipboardText(item.ItemName);
            }

            if (ImGui.Selectable("Open on GarlandTools"))
            {
                Dalamud.Utility.Util.OpenLink($"https://www.garlandtools.org/db/#item/{item.ItemId}");
            }

            // TODO: search on market

            ImGui.EndPopup();
        }
    }
}
