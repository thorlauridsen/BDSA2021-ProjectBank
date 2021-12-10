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
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationRepository _repository;

        public NotificationController(
            ILogger<NotificationController> logger,
            INotificationRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<NotificationDetailsDto>> Post(NotificationCreateDto notification)
        {
            var (status, created) = await _repository.CreateAsync(notification);
            if (status != Status.BadRequest)
            {
                return CreatedAtRoute(nameof(GetNotificationByUserId), new { userId = created?.UserOid }, created);
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet("{userId}", Name = "GetNotificationByUserId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationByUserId(string userId)
            => await _repository.GetNotificationsAsync(userId);

        [Authorize]
        [HttpPut("{notificationId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SeenNotification(int notificationId)
            => (await _repository.SeenNotificationAsync(notificationId)).ToActionResult();
    }
}
