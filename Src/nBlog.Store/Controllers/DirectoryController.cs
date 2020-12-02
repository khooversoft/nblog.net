using Microsoft.AspNetCore.Mvc;
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

        public DirectoryController(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            ArticleDirectory? record = await _directoryService.Get();
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ArticleDirectory record)
        {
            if (!record.IsValid()) return BadRequest();

            await _directoryService.Set(record);
            return Ok();
        }

        [HttpDelete()]
        public async Task<IActionResult> Delete()
        {
            bool status = await _directoryService.Delete();
            return status ? Ok() : NotFound();
        }
    }
}