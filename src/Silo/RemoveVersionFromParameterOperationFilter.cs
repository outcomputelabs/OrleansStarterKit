using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Silo
{
    public class RemoveVersionFromParameterOperationFilter : IOperationFilter
    {
        private const string VersionParameterName = "version";

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var parameter = operation.Parameters.SingleOrDefault(p => p.Name == VersionParameterName);
            if (parameter != null)
            {
                operation.Parameters.Remove(parameter);
            }
        }
    }
}
