using Microsoft.AspNetCore.Mvc;
using nBlog.sdk.Extensions;
using nBlog.sdk.Model;
using nBlog.sdk.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nBlog.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactRequestController : ControllerBase
    {
        private readonly IContactRequestStore _contactRequestStore;

        public ContactRequestController(IContactRequestStore contactRequestStore)
        {
            _contactRequestStore = contactRequestStore;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            ContactRequest? record = await _contactRequestStore.Get(id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactRequest record)
        {
            if (!record.IsValid()) return BadRequest();

            await _contactRequestStore.Set(record);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool status = await _contactRequestStore.Delete(id);
            return status ? Ok() : NotFound();
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] QueryParameters listParameters)
        {
            IReadOnlyList<string> list = await _contactRequestStore.List(listParameters);

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