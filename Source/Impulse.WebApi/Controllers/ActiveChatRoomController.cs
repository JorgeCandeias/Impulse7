using Impulse.Grains;
using Impulse.Models;
using Impulse.WebApi.Models;

namespace Impulse.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActiveChatRoomController : ControllerBase
{
    public ActiveChatRoomController(IMapper mapper, IGrainFactory factory)
    {
        _mapper = mapper;
        _factory = factory;
    }

    private readonly IMapper _mapper;
    private readonly IGrainFactory _factory;

    [HttpPut("member/{room}/{user}")]
    public async Task<ActionResult> Join([Required] string room, [Required] string user)
    {
        // for sample purposes we auto-create the user
        var chatUser = await _factory.GetChatUsersIndexGrain().GetOrAdd(user);

        // have the user join the chat room
        await _factory.GetActiveChatRoomGrain(room).Join(chatUser);

        return Ok($"User '{user}' joined '{room}'");
    }

    [HttpDelete("member/{room}/{user}")]
    public async Task<ActionResult> Leave([Required] string room, [Required] string user)
    {
        // for sample purposes we auto-create the user
        var chatUser = await _factory.GetChatUsersIndexGrain().GetOrAdd(user);

        // have the user leave the chat room
        await _factory.GetActiveChatRoomGrain(room).Leave(chatUser);

        return Ok($"User '{user}' left '{room}'");
    }

    [HttpGet("member/{room}")]
    public async Task<ActionResult<IEnumerable<ChatUserResponse>>> Users([Required] string room)
    {
        var result = await _factory.GetActiveChatRoomGrain(room).GetUsers();

        return Ok(_mapper.Map<IEnumerable<ChatUserResponse>>(result));
    }

    [HttpPost("message")]
    public async Task<ActionResult<ChatMessageResponse>> AddMessage([FromBody, Required] ActiveChatRoomAddMessageRequest request)
    {
        var message = _mapper.Map<ChatMessage>(request);

        await _factory.GetActiveChatRoomGrain(message.Room).Message(message);

        return Ok();
    }

    [HttpGet("message/{room}")]
    public async Task<ActionResult<IEnumerable<ChatMessageResponse>>> GetMessages([Required] string room)
    {
        var result = await _factory.GetActiveChatRoomGrain(room).GetMessages();

        return Ok(_mapper.Map<IEnumerable<ChatMessageResponse>>(result));
    }
}