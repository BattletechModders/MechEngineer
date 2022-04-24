using System;
using System.Collections.Generic;
using System.Linq;

namespace MechEngineer.Helper;

public static class LinqExtensions
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var comparer = new GeneralPropertyComparer<TSource, TKey>(keySelector);
        return source.Distinct(comparer);
    }

    private class GeneralPropertyComparer<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> expr { get; set; }
        public GeneralPropertyComparer(Func<T, TKey> expr)
        {
            this.expr = expr;
        }
        public bool Equals(T left, T right)
        {
            var leftProp = expr.Invoke(left);
            var rightProp = expr.Invoke(right);
            if (leftProp == null && rightProp == null)
            {
                return true;
            }
            else if ((leftProp == null) ^ (rightProp == null))
            {
                return false;
            }
            else
            {
                return leftProp.Equals(rightProp);
            }
        }
        public int GetHashCode(T obj)
        {
            var prop = expr.Invoke(obj);
            return prop == null ? 0 : prop.GetHashCode();
        }
    }
}