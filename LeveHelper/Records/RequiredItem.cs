using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LeveHelper;

public record RequiredItem
{
    public RequiredItem(RowRef<Item> Item, uint Amount)
    {
        this.Item = Item;
        this.Amount = Amount;
    }

    public RowRef<Item> Item { get; init; }
    public uint Amount { get; set; }
}
