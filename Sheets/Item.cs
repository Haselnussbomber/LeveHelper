using System.Collections.Generic;
using System.Linq;
using Dalamud.Memory;
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
    private bool? _isGatherable { get; set; } = null;
    private bool? _isFish { get; set; } = null;
    private bool? _isSpearfishing { get; set; } = null;
    private GatheringPoint[]? _gatheringPoints { get; set; } = null;
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
                : base.Name.ClearString() ?? "";
        }
    }

    public bool IsCrystal
        => ItemUICategory.Row == 59;

    public Recipe? Recipe
        => _recipe ??= Service.Data.GetExcelSheet<Recipe>()?.FirstOrDefault(recipe => recipe.ItemResult.Value?.RowId == RowId);

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

    public GatheringPoint[] GatheringPoints
        => _gatheringPoints ??= GatheringPointCache.FindByItemId(RowId) ?? Array.Empty<GatheringPoint>();

    public bool IsGatherable
        => _isGatherable ??= GatheringPoints.Any();

    public FishingSpot[] FishingSpots
        => _fishingSpots ??= FishingSpotCache.FindByItemId(RowId) ?? Array.Empty<FishingSpot>();

    public bool IsFish
        => _isFish ??= FishingSpots.Any();

    public bool IsSpearfishing
        => _isSpearfishing ??= Service.Data.GetExcelSheet<SpearfishingItem>()?.Any(row => row.Item.Row == RowId) ?? false;

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

                list.Add(new(Service.Data.GetExcelSheet<Item>()!.GetRow((uint)ingredient.ItemIngredient)!, ingredient.AmountIngredient));
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
