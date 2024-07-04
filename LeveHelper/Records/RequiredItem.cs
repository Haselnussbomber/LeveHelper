using LeveHelper.Sheets;

namespace LeveHelper;

public record RequiredItem
{
    public RequiredItem(LeveHelperItem Item, uint Amount)
    {
        this.Item = Item;
        this.Amount = Amount;
        AmountTotal = Amount;
    }

    public LeveHelperItem Item { get; init; }
    public uint Amount { get; set; }
    public uint AmountTotal { get; set; }
}
