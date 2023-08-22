using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

namespace LeveHelper.Structs;

[StructLayout(LayoutKind.Explicit, Size = 0x3EE0)]
public unsafe partial struct AddonItemSearch
{
    public enum SearchMode : uint
    {
        Normal = 0,
        ArmsFilter = 1,
        EquipmentFilter = 2,
        ItemsFilter = 3,
        HousingFilter = 4,
        Wishlist = 5,
        Favorites = 6,
    }

    [FieldOffset(0x2DB0)] public AtkComponentTextInput* TextInput;
    [FieldOffset(0x2DB0)] public HaselAtkComponentTextInput* HaselTextInput;
}

public unsafe partial struct AddonItemSearch
{
    public static partial class Addresses
    {
        public static readonly Address RunSearch = new("AddonItemSearch.RunSearch", "E8 ?? ?? ?? ?? 48 8B DE 48 8D BC 24 ?? ?? ?? ??", new ulong[] { 0xDE8B4800000000E8, 0x0000000024BC8D48 }, new ulong[] { 0xFFFFFF00000000FF, 0x00000000FFFFFFFF }, 0);
        public static readonly Address SetModeFilter = new("AddonItemSearch.SetModeFilter", "E8 ?? ?? ?? ?? EB 40 41 8D 40 FD ?? ?? ?? ?? ??", new ulong[] { 0x4140EB00000000E8, 0x0000000000FD408D }, new ulong[] { 0xFFFFFF00000000FF, 0x0000000000FFFFFF }, 0);
    }

    public static unsafe class MemberFunctionPointers
    {
        public static delegate* unmanaged[Stdcall]<AddonItemSearch*, bool, void> RunSearch => (delegate* unmanaged[Stdcall]<AddonItemSearch*, bool, void>)Addresses.RunSearch.Value;
        public static delegate* unmanaged[Stdcall]<AddonItemSearch*, SearchMode, int, void> SetModeFilter => (delegate* unmanaged[Stdcall]<AddonItemSearch*, SearchMode, int, void>)Addresses.SetModeFilter.Value;
    }

    public readonly void RunSearch(bool a2)
    {
        if (MemberFunctionPointers.RunSearch is null)
            throw new InvalidOperationException("Function pointer for AddonItemSearch.RunSearch is null. The resolver was either uninitialized or failed to resolve address with signature E8 ?? ?? ?? ?? 48 8B DE 48 8D BC 24 ?? ?? ?? ??.");

        fixed (AddonItemSearch* thisPtr = &this)
        {
            MemberFunctionPointers.RunSearch(thisPtr, a2);
        }
    }

    public readonly void SetModeFilter(SearchMode mode, int filter)
    {
        if (MemberFunctionPointers.SetModeFilter is null)
            throw new InvalidOperationException("Function pointer for AddonItemSearch.SetModeFilter is null. The resolver was either uninitialized or failed to resolve address with signature E8 ?? ?? ?? ?? EB 40 41 8D 40 FD ?? ?? ?? ?? ??.");

        fixed (AddonItemSearch* thisPtr = &this)
        {
            MemberFunctionPointers.SetModeFilter(thisPtr, mode, filter);
        }
    }
}
