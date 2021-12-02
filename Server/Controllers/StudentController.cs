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
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentRepository _repository;

        public StudentController(
            ILogger<StudentController> logger,
            IStudentRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IReadOnlyCollection<StudentDto>> Get()
            => await _repository.ReadAsync();

        [AllowAnonymous]
        [HttpGet("{userId}", Name = "GetByStudentId")]
        [ProducesResponseType(typeof(StudentDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<StudentDetailsDto>> GetByStudentId(int userId)
            => (await _repository.ReadAsync(userId)).ToActionResult();

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(StudentDetailsDto), 201)]
        public async Task<IActionResult> Post(StudentCreateDto student)
        {
            var created = await _repository.CreateAsync(student);
            return CreatedAtRoute(nameof(GetByStudentId), new { userId = created.Id }, created);
        }

        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int userId, [FromBody] StudentUpdateDto student)
            => (await _repository.UpdateAsync(userId, student)).ToActionResult();

        [Authorize]
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int userId)
            => (await _repository.DeleteAsync(userId)).ToActionResult();
    }
}
