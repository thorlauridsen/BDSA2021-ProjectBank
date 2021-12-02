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
        [ProducesResponseType(typeof(NotificationDetailsDto), 201)]
        public async Task<IActionResult> Post(NotificationCreateDto notification)
        {
            var created = await _repository.CreateAsync(notification);
            return CreatedAtRoute(nameof(GetNotificationByUserId), new { userId = created.Id }, created);
        }

        [Authorize]
        [HttpGet("{userId}", Name = "GetNotificationByUserId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationByUserId(int userId)
            => await _repository.GetNotificationsAsync(userId);

        [Authorize]
        [HttpPut("{notificationId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SeenNotification(int notificationId)
            => (await _repository.SeenNotificationAsync(notificationId)).ToActionResult();
    }
}
