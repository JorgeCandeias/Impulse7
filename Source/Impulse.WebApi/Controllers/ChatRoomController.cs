using Impulse.Grains;
using Impulse.Models.Exceptions;
using Impulse.WebApi.Models;

namespace Impulse.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatRoomController : ControllerBase
{
    public ChatRoomController(IGrainFactory factory, IMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    private readonly IGrainFactory _factory;
    private readonly IMapper _mapper;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatRoomResponse>>> Get()
    {
        var result = await _factory
            .GetChatRoomsIndexGrain()
            .GetAll();

        return Ok(_mapper.Map<IEnumerable<ChatRoomResponse>>(result));
    }

    [HttpGet("{guid}")]
    public async Task<ActionResult<ChatRoomResponse>> Get(Guid guid)
    {
        var result = await _factory
            .GetChatRoomsIndexGrain()
            .TryGetByGuid(guid);

        if (result is null)
        {
            return NotFound($"No chat room with guid '{guid}' was found");
        }

        return Ok(_mapper.Map<ChatRoomResponse>(result));
    }

    [HttpPost]
    public async Task<ActionResult<ChatRoomResponse>> Post(
        [FromBody, Required] ChatRoomCreateRequest request)
    {
        var result = await _factory
            .GetChatRoomsIndexGrain()
            .GetOrAdd(request.Name);

        return Ok(_mapper.Map<ChatRoomResponse>(result));
    }

    [HttpDelete("{guid}/{etag}")]
    public async Task<ActionResult> Delete(
        [Required] Guid guid,
        [Required] Guid etag)
    {
        try
        {
            await _factory
                .GetChatRoomsIndexGrain()
                .Remove(guid, etag);
        }
        catch (ConflictException ex)
        {
            return Conflict($"The provided ETag '{ex.CurrentETag}' is different from the stored version '{ex.StoredETag}'");
        }

        return Ok();
    }
}