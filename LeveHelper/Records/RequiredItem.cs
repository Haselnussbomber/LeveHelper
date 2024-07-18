using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public record RequiredItem
{
    public RequiredItem(Item Item, uint Amount)
    {
        this.Item = Item;
        this.Amount = Amount;
    }

    public Item Item { get; init; }
    public uint Amount { get; set; }
}
