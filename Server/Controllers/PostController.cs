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
        [ProducesResponseType(200)]
        public async Task<IReadOnlyCollection<PostDto>> Get()
            => await _repository.ReadAsync();

        [Authorize]
        [HttpGet("{postId:int}", Name = "GetByPostId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PostDetailsDto>> GetByPostId(int postId)
            => (await _repository.ReadAsync(postId)).ToActionResult();

        [Authorize]
        [HttpGet("tag/{tag}")]
        [ProducesResponseType(200)]
        public async Task<IReadOnlyCollection<PostDto>> GetByTag(string tag)
            => await _repository.ReadAsyncByTag(tag);

        [Authorize]
        [HttpGet("supervisor/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyCollection<PostDto>>> GetBySupervisor(string userId)
        {
            var (status, posts) = await _repository.ReadAsyncBySupervisor(userId);
            if (status == Status.NotFound) return NotFound();
            return Ok(posts);
        }

        [Authorize]
        [HttpGet("{postId}/comments")]
        [ProducesResponseType(200)]
        public async Task<IReadOnlyCollection<CommentDto>> GetComments(int postId)
        {
            var comments = await _repository.ReadAsyncComments(postId);
            return comments;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<PostDetailsDto>> Post(PostCreateDto post)
        {
            var (status, created) = await _repository.CreateAsync(post);
            if (status != Status.BadRequest)
            {
                return CreatedAtRoute(nameof(GetByPostId), new { postId = created?.Id }, created);
            }
            return BadRequest();
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPut("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int postId, [FromBody] PostUpdateDto post)
            => (await _repository.UpdateAsync(postId, post)).ToActionResult();

        [Authorize(Roles = "Supervisor")]
        [HttpDelete("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int postId)
            => (await _repository.DeleteAsync(postId)).ToActionResult();

        [Authorize]
        [HttpGet("{postId}")]
        public async Task<ActionResult<int>> IncrimentViewCount(int postId)
        {
            var (status, count) = (await _repository.IncrementViewCountAsync(postId));
            if (status != Status.BadRequest)
            {
                return Ok(count);
            }
            return BadRequest();
        }

    }
}
