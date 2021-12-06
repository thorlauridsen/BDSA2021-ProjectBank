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
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatRepository _repository;

        public ChatController(
            ILogger<ChatController> logger,
            IChatRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize]
        [HttpGet("{userId}", Name = "GetByChatId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IReadOnlyCollection<ChatDetailsDto>> GetByChatId(string userId)
            => await _repository.ReadAllChatsAsync(userId);

        [Authorize]
        [HttpGet("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IReadOnlyCollection<ChatMessageDto>> GetChatMessages(int userId)
            => await _repository.ReadSpecificChatAsync(userId);

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ChatDto), 201)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ChatDto>> Post(ChatCreateDto chat)
        {
            var (status, created) = await _repository.CreateNewChatAsync(chat);
            return CreatedAtRoute(nameof(GetByChatId), new { chatId = created?.ChatId }, created);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post(ChatMessageCreateDto chat)
            => (await _repository.CreateNewChatMessageAsync(chat)).ToActionResult();
    }
}
