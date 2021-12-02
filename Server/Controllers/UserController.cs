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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IReadOnlyCollection<UserDto>> Get()
            => await _repository.ReadAsync();

        [AllowAnonymous]
        [HttpGet("{userId}", Name = "GetByUserId")]
        [ProducesResponseType(typeof(UserDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailsDto>> GetByUserId(int userId)
            => (await _repository.ReadAsync(userId)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailsDto), 201)]
        public async Task<IActionResult> Post(UserCreateDto user)
        {
            var created = await _repository.CreateAsync(user);
            return CreatedAtRoute(nameof(GetByUserId), new { userId = created.Id }, created);
        }

        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int userId, [FromBody] UserUpdateDto user)
            => (await _repository.UpdateAsync(userId, user)).ToActionResult();

        [Authorize]
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int userId)
            => (await _repository.DeleteAsync(userId)).ToActionResult();
    }
}
