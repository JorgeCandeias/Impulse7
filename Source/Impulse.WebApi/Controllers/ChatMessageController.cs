using Impulse.Core;
using Impulse.Core.Exceptions;
using Impulse.Data;
using Impulse.WebApi.Models;

namespace Impulse.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatMessageController : ControllerBase
{
    public ChatMessageController(IMapper mapper, IChatMessageRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    private readonly IMapper _mapper;
    private readonly IChatMessageRepository _repository;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatMessageResponse>>> Get(CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAll(cancellationToken);

        return Ok(_mapper.Map<IEnumerable<ChatMessageResponse>>(result));
    }

    [HttpGet("{guid}")]
    public async Task<ActionResult<ChatMessageResponse>> Get(Guid guid, CancellationToken cancellationToken = default)
    {
        var result = await _repository.TryGetByGuid(guid, cancellationToken);

        if (result is null)
        {
            return NotFound($"No chat message with guid '{guid}' was found");
        }

        return Ok(_mapper.Map<ChatMessageResponse>(result));
    }

    [HttpGet("latest/{room}/{count}")]
    public async Task<ActionResult<ChatMessageResponse>> GetLatestCreatedByRoom([Required] string room, int count = 10, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetLatestCreatedByRoom(room, count, cancellationToken);

        return Ok(_mapper.Map<IEnumerable<ChatMessageResponse>>(result));
    }

    [HttpPost]
    public async Task<ActionResult<ChatMessageResponse>> Post([FromBody, Required] ChatMessageCreateRequest request, CancellationToken cancellationToken = default)
    {
        var message = _mapper.Map<ChatMessage>(request);

        var result = await _repository.Save(message, cancellationToken);

        return Ok(_mapper.Map<ChatMessageResponse>(result));
    }

    [HttpDelete("{guid}/{etag}")]
    public async Task<ActionResult> Delete([Required] Guid guid, [Required] Guid etag, CancellationToken cancellationToken = default)
    {
        try
        {
            await _repository.Remove(guid, etag, cancellationToken);
        }
        catch (ConflictException ex)
        {
            return Conflict($"The provided ETag '{ex.CurrentETag}' is different from the stored version '{ex.StoredETag}'");
        }

        return Ok();
    }
}