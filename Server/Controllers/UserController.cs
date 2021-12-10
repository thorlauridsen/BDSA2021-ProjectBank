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
        [ProducesResponseType(200)]
        public async Task<IReadOnlyCollection<UserDto>> Get()
            => await _repository.ReadAsync();

        [Authorize]
        [HttpGet("{userOid}", Name = "GetByUserOid")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailsDto>> GetByUserOid(string userOid)
            => (await _repository.ReadAsync(userOid)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<UserDetailsDto>> Post(UserCreateDto user)
        {
            var (status, created) = await _repository.CreateAsync(user);
            if (status != Status.BadRequest)
            {
                return CreatedAtRoute(nameof(GetByUserOid), new { userOid = created?.Oid }, created);
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{userOid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string userOid)
            => (await _repository.DeleteAsync(userOid)).ToActionResult();
    }
}
