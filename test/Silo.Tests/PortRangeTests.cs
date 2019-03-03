using Xunit;

namespace Silo.Tests
{
    public class PortRangeTests
    {
        [Fact]
        public void Holds_Data()
        {
            // arrange
            var start = 11111;
            var end = 22222;

            // act
            var range = new PortRange
            {
                Start = start,
                End = end
            };

            // assert
            Assert.Equal(start, range.Start);
            Assert.Equal(end, range.End);
        }
    }
}