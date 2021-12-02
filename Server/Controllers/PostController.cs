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
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostRepository _repository;

        public PostController(
            ILogger<PostController> logger,
            IPostRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IReadOnlyCollection<PostDto>> Get()
            => await _repository.ReadAsync();

        [AllowAnonymous]
        [HttpGet("{postId}")]
        [ProducesResponseType(typeof(PostDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PostDetailsDto>> Get(int postId)
            => (await _repository.ReadAsync(postId)).ToActionResult();

        [AllowAnonymous]
        [HttpGet("tag/{tag}")]
        public async Task<IReadOnlyCollection<PostDto>> GetByTag(string tag)
            => await _repository.ReadAsyncByTag(tag);

        [AllowAnonymous]
        [HttpGet("supervisor/{userId}")]
        public async Task<IReadOnlyCollection<PostDto>> GetBySupervisor(int userId)
            => await _repository.ReadAsyncBySupervisor(userId);

        [AllowAnonymous]
        [HttpGet("{postId}/comments")]
        public async Task<IReadOnlyCollection<CommentDto>> GetComments(int postId)
            => await _repository.ReadAsyncComments(postId);

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(PostDetailsDto), 201)]
        public async Task<IActionResult> Post(PostCreateDto post)
        {
            var created = await _repository.CreateAsync(post);
            return CreatedAtRoute(nameof(Get), new { created.Id }, created);
        }

        [Authorize]
        [HttpPut("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int postId, [FromBody] PostUpdateDto post)
            => (await _repository.UpdateAsync(postId, post)).ToActionResult();

        [Authorize]
        [HttpDelete("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int postId)
            => (await _repository.DeleteAsync(postId)).ToActionResult();
    }
}
