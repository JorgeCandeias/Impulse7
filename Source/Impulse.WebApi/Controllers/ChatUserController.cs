using AutoMapper;
using Impulse.Core.Exceptions;
using Impulse.Data;
using Impulse.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Impulse.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatUserController : ControllerBase
    {
        public ChatUserController(IGrainFactory factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

        private readonly IGrainFactory _factory;
        private readonly IMapper _mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatUserCreateResponse>>> Get()
        {
            var result = await _factory
                .GetChatUsersIndexGrain()
                .GetAll();

            return Ok(_mapper.Map<IEnumerable<ChatUserCreateResponse>>(result));
        }

        [HttpGet("{guid}")]
        public async Task<ActionResult<ChatUserCreateResponse>> Get(Guid guid)
        {
            var result = await _factory
                .GetChatUsersIndexGrain()
                .TryGetByGuid(guid);

            if (result is null)
            {
                return NotFound($"No chat user with guid '{guid}' was found");
            }

            return Ok(_mapper.Map<ChatUserCreateResponse>(result));
        }

        [HttpPost]
        public async Task<ActionResult<ChatUserCreateResponse>> Post(
            [FromBody, Required] ChatUserCreateRequest request)
        {
            var result = await _factory
                .GetChatUsersIndexGrain()
                .GetOrAdd(request.Name);

            return Ok(_mapper.Map<ChatUserCreateResponse>(result));
        }

        [HttpDelete("{guid}/{etag}")]
        public async Task<ActionResult> Delete(
            [Required] Guid guid,
            [Required] Guid etag)
        {
            try
            {
                await _factory
                    .GetChatUsersIndexGrain()
                    .Remove(guid, etag);
            }
            catch (ConflictException ex)
            {
                return Conflict($"The provided ETag '{ex.CurrentETag}' is different from the stored version '{ex.StoredETag}'");
            }

            return Ok();
        }
    }
}