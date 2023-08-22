using FFXIVClientStructs.Interop;

namespace LeveHelper.Structs;

public unsafe struct HaselAtkComponentTextInput
{
    public static class Addresses
    {
        public static readonly Address TriggerRedraw = new("AtkComponentTextInput.TriggerRedraw", "E8 ?? ?? ?? ?? 48 0F BF 56 ?? ?? ?? ?? ?? ?? ??", new ulong[] { 0xBF0F4800000000E8, 0x0000000000000056 }, new ulong[] { 0xFFFFFF00000000FF, 0x00000000000000FF }, 0);
    }

    public static unsafe class MemberFunctionPointers
    {
        public static delegate* unmanaged[Stdcall]<HaselAtkComponentTextInput*, nint> TriggerRedraw => (delegate* unmanaged[Stdcall]<HaselAtkComponentTextInput*, nint>)Addresses.TriggerRedraw.Value;
    }

    public readonly nint TriggerRedraw()
    {
        if (MemberFunctionPointers.TriggerRedraw is null)
            throw new InvalidOperationException("Function pointer for AtkComponentTextInput.TriggerRedraw is null. The resolver was either uninitialized or failed to resolve address with signature E8 ?? ?? ?? ?? 48 0F BF 56 ?? ?? ?? ?? ?? ?? ??.");

        fixed (HaselAtkComponentTextInput* thisPtr = &this)
        {
            return MemberFunctionPointers.TriggerRedraw(thisPtr);
        }
    }
}
