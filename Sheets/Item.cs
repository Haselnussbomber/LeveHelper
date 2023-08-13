using System.Collections.Generic;
using System.Linq;
using Dalamud.Memory;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Recipe = Lumina.Excel.GeneratedSheets.Recipe;
using SpearfishingItem = Lumina.Excel.GeneratedSheets.SpearfishingItem;

namespace LeveHelper.Sheets;

public class Item : Lumina.Excel.GeneratedSheets.Item
{
    private string _name { get; set; } = "";
    private Recipe? _recipe { get; set; } = null;
    private bool? _isCraftable { get; set; } = null;
    private uint? _resultAmount { get; set; } = null;
    private uint? _classJobIcon { get; set; } = null;
    private bool? _isFish { get; set; } = null;
    private bool? _isSpearfishing { get; set; } = null;
    private GatheringItem[]? _gatheringItems { get; set; } = null;
    private FishingSpot[]? _fishingSpots { get; set; } = null;
    private RequiredItem[]? _ingredients { get; set; } = null;
    private uint? _quantityOwned { get; set; } = null;
    private DateTime _quantityOwnedLastUpdate { get; set; }

    public new string Name
    {
        get
        {
            if (string.IsNullOrEmpty(_name))
            {
                unsafe
                {
                    var ptr = (nint)Framework.Instance()->GetUiModule()->GetRaptureTextModule()->FormatAddonText2(2021, (int)RowId, 0);
                    if (ptr != 0)
                        _name = MemoryHelper.ReadSeStringNullTerminated(ptr).ToString();
                }
            }

            return !string.IsNullOrEmpty(_name)
                ? _name
                : base.Name.ToDalamudString().ToString() ?? "";
        }
    }

    public bool IsCrystal
        => ItemUICategory.Row == 59;

    public Recipe? Recipe
        => _recipe ??= FindRow<Recipe>(recipe => recipe?.ItemResult.Value?.RowId == RowId);

    public bool IsCraftable
        => _isCraftable ??= Recipe != null;

    public uint ResultAmount
        => _resultAmount ??= _recipe?.AmountResult ?? 1;

    public uint? ClassJobIcon
    {
        get
        {
            if (_classJobIcon != null)
                return _classJobIcon;

            if (IsCraftable)
            {
                _classJobIcon ??= 62008 + Recipe!.CraftType.Row;
            }
            else if (IsGatherable)
            {
                _classJobIcon ??= GatheringPoints.First().Icon;
            }
            else if (IsFish)
            {
                _classJobIcon ??= FishingSpots.First().Icon;
            }

            return _classJobIcon;
        }
    }

    public GatheringItem[] GatheringItems
        => _gatheringItems ??= GetSheet<GatheringItem>().Where((row) => row.Item == RowId).ToArray();

    public GatheringPoint[] GatheringPoints
        => GatheringItems
            .SelectMany(row => row.GatheringPoints)
            .ToArray();

    public bool IsGatherable
        => GatheringPoints.Any();

    public FishingSpot[] FishingSpots
        => _fishingSpots ??= FishingSpotCache.FindByItemId(RowId) ?? Array.Empty<FishingSpot>();

    public bool IsFish
        => _isFish ??= FishingSpots.Any();

    public bool IsSpearfishing
        => _isSpearfishing ??= GetSheet<SpearfishingItem>().Any(row => row.Item.Row == RowId);

    public RequiredItem[] Ingredients
    {
        get
        {
            if (_ingredients != null)
                return _ingredients;

            if (!IsCraftable)
                return _ingredients ??= Array.Empty<RequiredItem>();

            var list = new List<RequiredItem>();

            foreach (var ingredient in Recipe!.UnkData5)
            {
                if (ingredient.ItemIngredient == 0 || ingredient.AmountIngredient == 0)
                    continue;

                list.Add(new(GetRow<Item>((uint)ingredient.ItemIngredient)!, ingredient.AmountIngredient));
            }

            return _ingredients = list.ToArray();
        }
    }

    public uint QuantityOwned
        => _quantityOwned ?? UpdateQuantityOwned();

    public unsafe uint UpdateQuantityOwned()
    {
        var inventoryManager = InventoryManager.Instance();

        _quantityOwned = 0
            + (uint)inventoryManager->GetInventoryItemCount(RowId)
            + (uint)inventoryManager->GetInventoryItemCount(RowId, true);

        return (uint)_quantityOwned;
    }

    public bool HasAllIngredients
        => Ingredients.All(ingredient => ingredient.Item.QuantityOwned > ingredient.Amount);

    public ItemQueueCategory QueueCategory
    {
        get
        {
#pragma warning disable IDE0046
            if (IsCrystal)
                return ItemQueueCategory.Crystals;

            if (IsGatherable || IsFish)
                return ItemQueueCategory.Gatherable;

            if (IsCraftable)
                return ItemQueueCategory.Craftable;

            return ItemQueueCategory.OtherSources;
#pragma warning restore IDE0046
        }
    }
}
