using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Silo.Tests.Fakes
{
    public class FakeHostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
