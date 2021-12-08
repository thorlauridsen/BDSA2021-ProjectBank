using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectBank.Core;
using ProjectBank.Server.Model;

namespace ProjectBank.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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

        [Authorize]
        [HttpGet]
        public async Task<IReadOnlyCollection<PostDto>> Get()
            => await _repository.ReadAsync();

        [Authorize]
        [HttpGet("{postId:int}", Name = "GetByPostId")]
        [ProducesResponseType(typeof(PostDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PostDetailsDto>> GetByPostId(int postId)
            => (await _repository.ReadAsync(postId)).ToActionResult();

        [Authorize]
        [HttpGet("tag/{tag}")]
        public async Task<IReadOnlyCollection<PostDto>> GetByTag(string tag)
            => await _repository.ReadAsyncByTag(tag);

        [Authorize]
        [HttpGet("supervisor/{userId}")]
        public async Task<IReadOnlyCollection<PostDto>> GetBySupervisor(string userId)
            => await _repository.ReadAsyncBySupervisor(userId);

        [Authorize]
        [HttpGet("{postId}/comments")]
        public async Task<IReadOnlyCollection<CommentDto>> GetComments(int postId)
        {
            var (status, comments) = await _repository.ReadAsyncComments(postId);
            return status.Equals(Status.Success) ? comments : null;
        }

        [Authorize(Roles = "supervisor")]
        [HttpPost]
        [ProducesResponseType(typeof(PostDetailsDto), 201)]
        public async Task<ActionResult<PostDetailsDto>> Post(PostCreateDto post)
        {
            var (status, created) = await _repository.CreateAsync(post);
            if (status != Status.BadRequest)
            {
                return CreatedAtRoute(nameof(GetByPostId), new { postId = created?.Id }, created);
            }
            return BadRequest();
        }

        [Authorize(Roles = "supervisor")]
        [HttpPut("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int postId, [FromBody] PostUpdateDto post)
            => (await _repository.UpdateAsync(postId, post)).ToActionResult();

        [Authorize(Roles = "supervisor")]
        [HttpDelete("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int postId)
            => (await _repository.DeleteAsync(postId)).ToActionResult();
    }
}
