using System;
namespace BuildingBlocks.Common.Extensions;

public static class CollectionExtension
{
    public static bool IsEmpty<T>(this ICollection<T> @this)
    {
        return @this.Count == 0;
    }

    public static bool IsNotEmpty<T>(this ICollection<T> @this)
    {
        return @this.Count != 0;
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> @this)
    {
        return @this == null || @this.Count == 0;
    }

    public static bool IsNotNullNorEmpty<T>(this ICollection<T> @this)
    {
        return @this != null && @this.Count != 0;
    }

    public static bool AddIf<T>(this ICollection<T> @this, Func<T, bool> predicate, T value)
    {
        if (!predicate(value)) return false;
        @this.Add(value);
        return true;
    }

    public static bool AddIfNotContains<T>(this ICollection<T> @this, T value)
    {
        if (@this.Contains(value)) return false;
        @this.Add(value);
        return true;
    }

    public static void AddRange<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (T value in values)
        {
            @this.Add(value);
        }
    }

    
    public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
    {
        foreach (T value in values)
        {
            if (predicate(value))
            {
                @this.Add(value);
            }
        }
    }

    public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (T value in values)
        {
            if (!@this.Contains(value))
            {
                @this.Add(value);
            }
        }
    }

    
    public static bool ContainsAll<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (T value in values)
        {
            if (!@this.Contains(value))
            {
                return false;
            }
        }

        return true;
    }

   
    public static bool ContainsAny<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (T value in values)
        {
            if (@this.Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    
    public static void RemoveIf<T>(this ICollection<T> @this, T value, Func<T, bool> predicate)
    {
        if (predicate(value))
        {
            @this.Remove(value);
        }
    }

    public static void RemoveIfContains<T>(this ICollection<T> @this, T value)
    {
        if (@this.Contains(value))
        {
            @this.Remove(value);
        }
    }

    public static void RemoveRange<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (T value in values)
        {
            @this.Remove(value);
        }
    }

    public static void RemoveRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
    {
        foreach (T value in values)
        {
            if (predicate(value))
            {
                @this.Remove(value);
            }
        }
    }

    public static void RemoveRangeIfContains<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (T value in values)
        {
            if (@this.Contains(value))
            {
                @this.Remove(value);
            }
        }
    }

    
    public static void RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> predicate)
    {
        List<T> list = @this.Where(predicate).ToList();
        foreach (T item in list)
        {
            @this.Remove(item);
        }
    }

    public static List<T> AppendItem<T>(this List<T> list, T item)
    {
        list.Add(item);

        return list;
    }

    public static List<T> RemoveItem<T>(this List<T> list, T item)
    {
        list.Remove(item);

        return list;
    }
}

