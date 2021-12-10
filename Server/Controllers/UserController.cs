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
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _repository;

        public UserController(
            ILogger<UserController> logger,
            IUserRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<UserDto>), 200)]
        public async Task<IReadOnlyCollection<UserDto>> Get()
            => await _repository.ReadAsync();

        [Authorize]
        [HttpGet("{userId}", Name = "GetByUserId")]
        [ProducesResponseType(typeof(UserDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailsDto>> GetByUserId(string userId)
            => (await _repository.ReadAsync(userId)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailsDto), 201)]
        public async Task<ActionResult<UserDetailsDto>> Post(UserCreateDto user)
        {
            var (status, created) = await _repository.CreateAsync(user);
            if (status != Status.BadRequest)
            {
                return CreatedAtRoute(nameof(GetByUserId), new { userId = created?.oid }, created);
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string userId)
            => (await _repository.DeleteAsync(userId)).ToActionResult();
    }
}
