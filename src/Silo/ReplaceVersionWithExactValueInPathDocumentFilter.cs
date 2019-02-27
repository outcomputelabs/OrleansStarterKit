using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Silo
{
    public class ReplaceVersionWithExactValueInPathDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths = swaggerDoc.Paths.ToDictionary(
                path => path.Key.Replace("{version}", swaggerDoc.Info.Version),
                path => path.Value);
        }
    }
}
