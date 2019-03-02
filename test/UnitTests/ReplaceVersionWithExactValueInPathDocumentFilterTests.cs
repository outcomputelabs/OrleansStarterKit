using Silo;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{
    public class ReplaceVersionWithExactValueInPathDocumentFilterTests
    {
        [Fact]
        public void Applies_Filter()
        {
            // arrange
            var filter = new ReplaceVersionWithExactValueInPathDocumentFilter();
            var document = new SwaggerDocument()
            {
                Info = new Info
                {
                    Version = "123"
                },
                Paths = new Dictionary<string, PathItem>
                {
                    { "{version}", null }
                }
            };

            // act
            filter.Apply(document, null);

            // assert
            Assert.Single(document.Paths.Keys, document.Info.Version);
        }
    }
}
