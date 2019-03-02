using Silo;
using Xunit;

namespace UnitTests
{
    public class ValueExtensionsTests
    {
        [Fact]
        public void ValueIf_Replaces_Values()
        {
            // arrange
            var value = 1;
            var compare = 1;
            var replace = 2;

            // act
            var actual = value.ValueIf(compare, replace);

            // assert
            Assert.Equal(replace, actual);
        }
    }
}
