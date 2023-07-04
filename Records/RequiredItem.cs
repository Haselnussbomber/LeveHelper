using LeveHelper.Sheets;

namespace LeveHelper;

public record RequiredItem
{
    public RequiredItem(Item Item, uint Amount)
    {
        this.Item = Item;
        this.Amount = Amount;
        AmountTotal = Amount;
    }

    public Item Item { get; init; }
    public uint Amount { get; set; }
    public uint AmountTotal { get; set; }
}
