using AutoMapper;
using Impulse.Data;
using Impulse.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IEnumerable<ApiChatUser>> Get()
        {
            var result = await _factory
                .GetChatRoomsIndexGrain()
                .GetAll();

            return _mapper.Map<IEnumerable<ApiChatUser>>(result);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}