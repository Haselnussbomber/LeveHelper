using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Memory;

namespace LeveHelper;

public unsafe class MemoryUtils
{
    public static byte* FromByteArray(byte[] data)
    {
        var len = data.Length;
        var ptr = Marshal.AllocHGlobal(len + 1);
        Unsafe.InitBlockUnaligned((byte*)ptr, 0, (uint)len + 1);
        Marshal.Copy(data, 0, ptr, len);
        return (byte*)ptr;
    }

    public static byte* FromString(string str)
    {
        var len = Encoding.UTF8.GetByteCount(str) + 1;
        var ptr = Marshal.AllocHGlobal(len);
        Unsafe.InitBlockUnaligned((byte*)ptr, 0, (uint)len);
        MemoryHelper.WriteString(ptr, str);
        return (byte*)ptr;
    }

    public static int strlen(byte* ptr)
    {
        int i;
        for (i = 0; *(bool*)((nint)ptr + i); i++) ;
        return i;
    }

    public static byte* strconcat(params byte*[] ptrs)
    {
        var lengths = new int[ptrs.Length];
        var totalLength = 0;

        for (var i = 0; i < ptrs.Length; i++)
        {
            var len = strlen(ptrs[i]);
            lengths[i] = len;
            totalLength += len;
        }

        var outPtr = Marshal.AllocHGlobal(totalLength + 1);
        var offset = 0;
        for (var i = 0; i < ptrs.Length; i++)
        {
            var len = lengths[i];
            Buffer.MemoryCopy(ptrs[i], (void*)(outPtr + offset), len, len);
            offset += len;
        }
        *(byte*)(outPtr + totalLength) = 0;
        return (byte*)outPtr;
    }
}
