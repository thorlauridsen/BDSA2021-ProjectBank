using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectBank.Core;
using ProjectBank.Server.Model;

namespace ProjectBank.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentRepository _repository;

        public CommentController(
            ILogger<CommentController> logger,
            ICommentRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IReadOnlyCollection<CommentDto>> Get()
            => await _repository.ReadAsync();

        [AllowAnonymous]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDetailsDto>> Get(int id)
            => (await _repository.ReadAsync(id)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(CommentDetailsDto), 201)]
        public async Task<IActionResult> Post(CommentCreateDto comment)
        {
            var created = await _repository.CreateAsync(comment);

            return CreatedAtRoute(nameof(Get), new { created.Id }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] CommentUpdateDto comment)
               => (await _repository.UpdateAsync(id, comment)).ToActionResult();

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
              => (await _repository.DeleteAsync(id)).ToActionResult();
    }
}
