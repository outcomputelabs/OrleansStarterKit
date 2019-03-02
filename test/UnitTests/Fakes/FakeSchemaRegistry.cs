using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace UnitTests.Fakes
{
    public class FakeSchemaRegistry : ISchemaRegistry
    {
        public IDictionary<string, Schema> Definitions => throw new NotSupportedException();

        public Schema GetOrRegister(Type type)
        {
            throw new NotSupportedException();
        }
    }
}
