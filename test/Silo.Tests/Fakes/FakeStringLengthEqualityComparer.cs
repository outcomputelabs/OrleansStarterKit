using System.Collections.Generic;

namespace Silo.Tests.Fakes
{
    public class FakeStringLengthEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return EqualityComparer<int>.Default.Equals(x.Length, y.Length);
        }

        public int GetHashCode(string obj)
        {
            return EqualityComparer<int>.Default.GetHashCode(obj.Length);
        }
    }
}
