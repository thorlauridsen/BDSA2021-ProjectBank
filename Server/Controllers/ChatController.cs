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
        [HttpGet("{chatId}/{userId}", Name = "GetByChatId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ChatDetailsDto?> GetByChatId(int chatId, string userId)
            => await _repository.ReadChatAsync(chatId, userId);

        [Authorize]
        [HttpGet("message/{messageId}", Name = "GetByMessageId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ChatMessageDto?> GetByMessageId(int messageId)
    => await _repository.ReadSpecificMessageAsync(messageId);

        [Authorize]
        [HttpGet("user/{userId}", Name = "GetChatsByUserId")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IReadOnlyCollection<ChatDetailsDto>> GetChatsByUserId(string userId)
            => await _repository.ReadAllChatsAsync(userId);

        [Authorize]
        [HttpGet("{chatId}/messages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IReadOnlyCollection<ChatMessageDto>> GetChatMessages(int chatId)
            => await _repository.ReadSpecificChatAsync(chatId);

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ChatDto>> Post(ChatCreateDto chat)
        {
            var (status, created) = await _repository.CreateNewChatAsync(chat);
            if (created == null) return new BadRequestResult();
            return CreatedAtRoute(nameof(GetByChatId), new { chatId = created?.ChatId, userId = chat.FromUserId }, created);
        }

        [Authorize]
        [HttpPost("message")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ChatMessageDetailsDto>> Post(ChatMessageCreateDto chat)
        {
            var (status, created) = await _repository.CreateNewChatMessageAsync(chat);
            return CreatedAtRoute(nameof(GetByMessageId), new { messageId = created.chatMessageId }, created);
        }
    }
}
