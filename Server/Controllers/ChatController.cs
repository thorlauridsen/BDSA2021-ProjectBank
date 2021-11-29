using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectBank.Core;

namespace ProjectBank.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
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

    [AllowAnonymous]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    [HttpGet("{id}")]
    public async Task<IReadOnlyCollection<ChatDetailsDto>> Get(int userId)
        => await _repository.ReadAllChatsAsync(userId);

    [AllowAnonymous]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    [HttpGet("{id}")]
    public async Task<IReadOnlyCollection<ChatMessageDto>> GetMessagesInChat(int userId)
    => await _repository.ReadChatAsync(userId);




}