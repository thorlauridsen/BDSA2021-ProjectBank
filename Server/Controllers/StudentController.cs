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
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentRepository _repository;

        public StudentController(ILogger<StudentController> logger, IStudentRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IReadOnlyCollection<StudentDto>> Get()
            => await _repository.ReadAsync();

        [AllowAnonymous]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(StudentDetailsDto), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDetailsDto>> Get(int id)
            => (await _repository.ReadAsync(id)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(StudentDetailsDto), 201)]
        public async Task<IActionResult> Post(StudentCreateDto student)
        {
            var created = await _repository.CreateAsync(student);

            return CreatedAtRoute(nameof(Get), new { created.Id }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] StudentUpdateDto student)
               => (await _repository.UpdateAsync(id, student)).ToActionResult();

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
              => (await _repository.DeleteAsync(id)).ToActionResult();
    }
}
