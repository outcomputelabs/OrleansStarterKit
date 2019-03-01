using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;

namespace UnitTests.Fakes
{
    public class FakeApiVersionDescriptionProvider : IApiVersionDescriptionProvider
    {
        public IReadOnlyList<ApiVersionDescription> ApiVersionDescriptions => throw new NotImplementedException();

        public bool IsDeprecated(ActionDescriptor actionDescriptor, ApiVersion apiVersion)
        {
            throw new NotImplementedException();
        }
    }
}
