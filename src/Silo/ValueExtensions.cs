using System.Collections.Generic;

namespace Silo
{
    public static class ValueExtensions
    {
        public static T ValueIf<T>(this T value, T compare, T replace, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            return comparer.Equals(value, compare) ? replace : value;
        }
    }
}
