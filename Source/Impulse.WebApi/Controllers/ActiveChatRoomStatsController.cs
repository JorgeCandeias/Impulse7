using Impulse.Grains;
using Impulse.WebApi.Models;

namespace Impulse.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActiveChatRoomStatsController : ControllerBase
{
    public ActiveChatRoomStatsController(IMapper mapper, IGrainFactory factory)
    {
        _mapper = mapper;
        _factory = factory;
    }

    private readonly IMapper _mapper;
    private readonly IGrainFactory _factory;

    [HttpGet("local")]
    public async Task<ActiveChatRoomSiloStatsResponse> GetLocalStats()
    {
        var result = await _factory.GetActiveChatRoomLocalStatsGrain().GetStats();

        return _mapper.Map<ActiveChatRoomSiloStatsResponse>(result);
    }

    [HttpGet("global")]
    public async Task<ActiveChatRoomClusterStatsResponse> GetGlobalStats()
    {
        var result = await _factory.GetActiveChatRoomGlobalStatsReplicaGrain().GetStats();

        return _mapper.Map<ActiveChatRoomClusterStatsResponse>(result);
    }
}