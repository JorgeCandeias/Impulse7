using Impulse.Grains;

namespace Impulse.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiagnosticsController : ControllerBase
{
    public DiagnosticsController(IGrainFactory factory)
    {
        _factory = factory;
    }

    private readonly IGrainFactory _factory;

    [HttpPut("trace/messages/{room}")]
    public async Task StartTracingMessages([Required] string room)
    {
        await _factory.GetActiveChatRoomTracingGrain(room).Start();
    }

    [HttpDelete("trace/messages/{room}")]
    public async Task StopTracingMessages([Required] string room)
    {
        await _factory.GetActiveChatRoomTracingGrain(room).Stop();
    }
}