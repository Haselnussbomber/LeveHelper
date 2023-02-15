using System;

namespace LeveHelper;

public record QueuedItem
{
    public QueuedItem(CachedItem Item, uint AmountNeeded)
    {
        this.Item = Item;
        this.AmountNeeded = AmountNeeded;

        AmountHave = Item.QuantityOwned;
    }

    public CachedItem Item { get; init; }
    public uint AmountHave { get; set; }
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
        return $"QueuedItem({Item.ItemName}, Have: {AmountHave}, Needed: {AmountNeeded}, Left: {AmountLeft})";
    }
}
