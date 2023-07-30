using Impulse.Core;

namespace Impulse.WebApi.Models;

internal class ApiModelsProfile : Profile
{
    public ApiModelsProfile()
    {
        CreateMap<ChatUser, ChatUserCreateResponse>();

        CreateMap<ChatRoom, ChatRoomCreateResponse>();
    }
}