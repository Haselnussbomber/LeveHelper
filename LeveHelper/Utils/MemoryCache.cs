using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using HaselCommon.Extensions;

namespace LeveHelper.Utils;

public abstract class MemoryCache<TKey, TValue> where TKey : notnull, IEquatable<TKey>
{
    protected readonly ConcurrentDictionary<TKey, TValue?> Data = [];

    public virtual void Dispose()
    {
        Clear();
    }

    public abstract TValue? CreateEntry(TKey key);

    public virtual TValue? GetValue(TKey key)
    {
        TryGetValue(key, out var value);
        return value;
    }

    public virtual bool TryGetValue(TKey key, [NotNullWhen(returnValue: true)] out TValue? value)
    {
        if (Data.TryGetValue(key, out var _value))
        {
            if (_value == null)
            {
                value = default;
                return false;
            }

            value = _value!;
            return true;
        }

        var entry = CreateEntry(key);

        Data.TryAdd(key, entry);

        value = entry;
        return entry != null;
    }

    public virtual bool Remove(TKey key)
    {
        return Data.TryRemove(key, out var _);
    }

    public virtual void Clear()
    {
        Data.Dispose();
    }
}
