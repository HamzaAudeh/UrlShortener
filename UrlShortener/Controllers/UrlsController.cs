using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Route("api/")]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlShorteningService _service;
        public UrlsController(IUrlShorteningService service)
        {
            _service = service;
        }

        [HttpPost("shorten/url")]
        public async Task<ActionResult> Shorten(string url)
        {
            string shortnedUrl = await _service.Shorten(url);
            return Ok(shortnedUrl);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult> GetFullUrl([FromRoute] string code)
        {
            var fullUrl = await _service.GetFullUrl(code);
            return Redirect(fullUrl);
        }
    }
}
