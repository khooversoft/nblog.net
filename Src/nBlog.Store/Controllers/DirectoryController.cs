using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nBlog.sdk.Actors.Directory;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using System.Threading.Tasks;

namespace nBlog.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<DirectoryController> _logger;

        public DirectoryController(IDirectoryService directoryService, ILogger<DirectoryController> logger)
        {
            _directoryService = directoryService;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            ArticleDirectory? record = await _directoryService.Get();
            if (record == null) return NotFound();

            _logger.LogTrace($"{nameof(Get)}");
            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ArticleDirectory record)
        {
            if (!record.IsValid()) return BadRequest();

            _logger.LogTrace($"{nameof(Post)}");

            await _directoryService.Set(record);
            return Ok();
        }

        [HttpDelete()]
        public async Task<IActionResult> Delete()
        {
            _logger.LogTrace($"{nameof(Delete)}");

            bool status = await _directoryService.Delete();
            return status ? Ok() : NotFound();
        }
    }
}