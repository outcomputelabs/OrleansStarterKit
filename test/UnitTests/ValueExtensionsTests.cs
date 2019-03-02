using Silo;
using UnitTests.Fakes;
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

        [Fact]
        public void ValueIf_Replaces_Values_With_Comparer()
        {
            // arrange
            var value = "123";
            var compare = "234";
            var replace = "345";

            // act
            var actual = value.ValueIf(compare, replace, new FakeStringLengthEqualityComparer());

            // assert
            Assert.Equal(replace, actual);
        }

        [Fact]
        public void ValueIf_Skips_Replacement()
        {
            // arrange
            var value = 1;
            var compare = 2;
            var replace = 3;

            // act
            var actual = value.ValueIf(compare, replace);

            // assert
            Assert.Equal(value, actual);
        }
    }
}
