using System;
using LeveHelper.Sheets;

namespace LeveHelper;

public record QueuedItem
{
    public QueuedItem(Item Item, uint AmountNeeded)
    {
        this.Item = Item;
        this.AmountNeeded = AmountNeeded;
    }

    public Item Item { get; init; }
    public uint AmountHave => Item.QuantityOwned;
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
