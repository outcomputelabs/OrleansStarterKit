using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Silo.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;

namespace Silo
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly SupportApiOptions _options;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<SupportApiOptions> options)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public void Configure(SwaggerGenOptions options)
        {
            // add a new swagger version page for each api version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new Info
                {
                    Title = _options.Title,
                    Version = description.ApiVersion.ToString()
                });
            }

            // apply a swagger fix to remove the version number as a parameter
            options.OperationFilter<RemoveVersionFromParameterOperationFilter>();

            // apply a swagger fix to show the correct version in the endpoint description
            options.DocumentFilter<ReplaceVersionWithExactValueInPathDocumentFilter>();

            // add swagger xml comments
            options.IncludeXmlComments(
                Path.Combine(
                    AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            options.EnableAnnotations();
        }
    }
}
