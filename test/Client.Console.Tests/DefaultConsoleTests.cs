using System.IO;
using Xunit;

namespace Client.Console.Tests
{
    public class DefaultConsoleTests
    {
        [Fact]
        public void WriteLine_WritesToSystemConsole()
        {
            var console = new DefaultConsole();
            using (var output = new MemoryStream())
            using (var writer = new StreamWriter(output))
            using (var reader = new StreamReader(output))
            {
                System.Console.SetOut(writer);

                console.WriteLine("SomeFormatString {0}", 123);
                writer.Flush();

                output.Position = 0;

                Assert.Equal("SomeFormatString 123", reader.ReadLine());
            }
        }
    }
}
