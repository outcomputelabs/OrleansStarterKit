using System.Collections.Generic;

namespace Silo
{
    public static class ValueExtensions
    {
        public static T ValueIf<T>(this T value, T compare, T replace)
        {
            return ValueIf(value, compare, replace, EqualityComparer<T>.Default);
        }

        public static T ValueIf<T>(this T value, T compare, T replace, IEqualityComparer<T> comparer)
        {
            return comparer.Equals(value, compare) ? replace : value;
        }
    }
}
