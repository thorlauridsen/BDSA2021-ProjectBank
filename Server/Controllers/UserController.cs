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
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(UserDetailsDto), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsDto>> Get(int id)
            => (await _repository.ReadAsync(id)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailsDto), 201)]
        public async Task<IActionResult> Post(UserCreateDto user)
        {
            var created = await _repository.CreateAsync(user);

            return CreatedAtRoute(nameof(Get), new { created.Id }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDto user)
               => (await _repository.UpdateAsync(id, user)).ToActionResult();

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
              => (await _repository.DeleteAsync(id)).ToActionResult();
    }
}
