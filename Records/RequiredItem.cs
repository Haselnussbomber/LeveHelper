namespace LeveHelper;

public record RequiredItem
{
    public RequiredItem(CachedItem Item, uint Amount)
    {
        this.Item = Item;
        this.Amount = Amount;
        AmountTotal = Amount;
    }

    public CachedItem Item { get; init; }
    public uint Amount { get; set; }
    public uint AmountTotal { get; set; }
}
