using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pacco.Services.Customers.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Customers2Controller : ControllerBase
    {
        [HttpGet(nameof(test))]
        public async Task<ActionResult> test()
        {
            return null;
        }
    }
}