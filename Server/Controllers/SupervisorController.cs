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
    public class SupervisorController : ControllerBase
    {
        private readonly ILogger<SupervisorController> _logger;
        private readonly ISupervisorRepository _repository;

        public SupervisorController(
            ILogger<SupervisorController> logger,
            ISupervisorRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IReadOnlyCollection<SupervisorDto>> Get()
            => await _repository.ReadAsync();

        [AllowAnonymous]
        [HttpGet("{userId}", Name = "GetBySupervisorId")]
        [ProducesResponseType(typeof(SupervisorDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SupervisorDetailsDto>> GetBySupervisorId(int userId)
            => (await _repository.ReadAsync(userId)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(SupervisorDetailsDto), 201)]
        public async Task<IActionResult> Post(SupervisorCreateDto supervisor)
        {
            var created = await _repository.CreateAsync(supervisor);
            return CreatedAtRoute(nameof(GetBySupervisorId), new { created.Id }, created);
        }

        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int userId, [FromBody] SupervisorUpdateDto supervisor)
            => (await _repository.UpdateAsync(userId, supervisor)).ToActionResult();

        [Authorize]
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int userId)
            => (await _repository.DeleteAsync(userId)).ToActionResult();
    }
}
