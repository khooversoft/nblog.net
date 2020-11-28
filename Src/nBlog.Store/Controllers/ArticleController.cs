using Microsoft.AspNetCore.Mvc;
using nBlog.sdk.Model;
using nBlog.sdk.Services;
using nBlog.sdk.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Azure.DataLake;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace nBlog.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleStoreService _acticleStoreService;

        public ArticleController(IArticleStoreService acticleStoreService)
        {
            _acticleStoreService = acticleStoreService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            ArticlePayload? record = await _acticleStoreService.Get(id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ArticlePayload record)
        {
            await _acticleStoreService.Set(record);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            bool status = await _acticleStoreService.Delete(id);
            return status ? Ok() : NotFound();
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] QueryParameters listParameters)
        {
            IReadOnlyList<string> list = await _acticleStoreService.List(listParameters);

            var result = new BatchSet<string>
            {
                QueryParameters = listParameters,
                NextIndex = listParameters.Index + listParameters.Count,
                Records = list.ToArray(),
            };

            return Ok(result);
        }
    }
}
