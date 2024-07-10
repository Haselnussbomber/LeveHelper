using FFXIVClientStructs.FFXIV.Client.Game;
using HaselCommon.Caching;

namespace LeveHelper.Caches;

public class ItemQuantityCache : MemoryCache<uint, uint>
{
    public override unsafe uint CreateEntry(uint itemId)
    {
        var inventoryManager = InventoryManager.Instance();
        return (uint)inventoryManager->GetInventoryItemCount(itemId) + (uint)inventoryManager->GetInventoryItemCount(itemId, true);
    }
}
