using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn.WebAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(1.0)]
    [AllowAnonymous]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> logger;

        public NewsController(ILogger<NewsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "News";
        }
    }
}
