using LeveHelper.Services;
using Lumina.Excel.Sheets;

namespace LeveHelper;

public record QueuedItem
{
    private readonly ExtendedItemService ExtendedItemService;

    public QueuedItem(Item Item, uint AmountNeeded, ExtendedItemService ExtendedItemService)
    {
        this.Item = Item;
        this.AmountNeeded = AmountNeeded;
        this.ExtendedItemService = ExtendedItemService;
    }

    public Item Item { get; init; }
    public uint AmountHave => ExtendedItemService.GetQuantity(Item.RowId);
    public uint AmountNeeded { get; set; }
    public uint AmountLeft
    {
        get
        {
            var left = (int)AmountNeeded - (int)AmountHave;
            return left > 0 ? (uint)Math.Abs(left) : 0;
        }
    }

    public override string ToString()
    {
        return $"QueuedItem({Item.Name}, Have: {AmountHave}, Needed: {AmountNeeded}, Left: {AmountLeft})";
    }
}
