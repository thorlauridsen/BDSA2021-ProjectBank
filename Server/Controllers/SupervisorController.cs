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
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(SupervisorDetailsDto), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<SupervisorDetailsDto>> Get(int id)
            => (await _repository.ReadAsync(id)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(SupervisorDetailsDto), 201)]
        public async Task<IActionResult> Post(SupervisorCreateDto supervisor)
        {
            var created = await _repository.CreateAsync(supervisor);

            return CreatedAtRoute(nameof(Get), new { created.Id }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] SupervisorUpdateDto supervisor)
               => (await _repository.UpdateAsync(id, supervisor)).ToActionResult();

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
              => (await _repository.DeleteAsync(id)).ToActionResult();
    }
}
