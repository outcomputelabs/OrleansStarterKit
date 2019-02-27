using Microsoft.AspNetCore.Mvc;

namespace Silo.Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DummyController : ControllerBase
    {
    }
}
