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

        [Authorize]
        [HttpGet("{postId}/{commentId}", Name = "GetByCommentId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CommentDetailsDto>> GetByCommentId(int postId, int commentId)
            => (await _repository.ReadAsync(postId, commentId)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommentDetailsDto>> Post(CommentCreateDto comment)
        {
            var (status, created) = await _repository.CreateAsync(comment);
            if (status != Status.BadRequest)
            {
                return CreatedAtRoute(nameof(GetByCommentId), new { postId = comment.postid, commentId = created?.Id }, created);
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{postId}/{commentId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int postId, int commentId)
            => (await _repository.DeleteAsync(postId, commentId)).ToActionResult();
    }
}
