using System;
using UnityEngine;

namespace MechEngineer.Features.AutoFix;

public class TonnageChange : ValueChange<float>
{
}

public class SlotChange : ValueChange<int>
{
}

public class ValueChange<T> where T : struct, IComparable<T>, IEquatable<T>
{
    public T From;
    public T By;

    public bool FromIsMin;
    public T? NewMin;

    internal T? Change(T originalValue)
    {
        if (!FromIsMin && !Equals(originalValue, From))
        {
            return null;
        }

        var newValue = Add(originalValue, By);

        if (NewMin.HasValue)
        {
            var min = NewMin.Value;
            if (newValue.CompareTo(min) < 0)
            {
                return min;
            }
        }

        return newValue;
    }

    private static bool Equals(T a, T b)
    {
        if (typeof(T) == typeof(float))
        {
            var a1 = a as float?;
            var b1 = b as float?;
            if (a1.HasValue && b1.HasValue)
            {
                return Mathf.Approximately(a1.Value, b1.Value);
            }
        }
        else if (typeof(T) == typeof(int))
        {
            var a1 = a as int?;
            var b1 = b as int?;
            if (a1.HasValue && b1.HasValue)
            {
                return a1.Value == b1.Value;
            }
        }

        return false;
    }

    private static T Add(T a, T b)
    {
        if (typeof(T) == typeof(float))
        {
            var a1 = a as float?;
            var b1 = b as float?;
            if (a1.HasValue && b1.HasValue)
            {
                var c1 = a1.Value + b1.Value;
                var c = c1 as T?;
                if (c.HasValue)
                {
                    return c.Value;
                }
            }
        }
        else if (typeof(T) == typeof(int))
        {
            var a1 = a as int?;
            var b1 = b as int?;
            if (a1.HasValue && b1.HasValue)
            {
                var c1 = a1.Value + b1.Value;
                var c = c1 as T?;
                if (c.HasValue)
                {
                    return c.Value;
                }
            }
        }

        return default;
    }
}