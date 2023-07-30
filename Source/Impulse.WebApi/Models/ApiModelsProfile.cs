using Impulse.Core;

namespace Impulse.WebApi.Models;

internal class ApiModelsProfile : Profile
{
    public ApiModelsProfile()
    {
        CreateMap<ChatUser, ChatUserResponse>();

        CreateMap<ChatRoom, ChatRoomResponse>();

        CreateMap<ChatMessage, ChatMessageResponse>();

        CreateMap<ChatMessageCreateRequest, ChatMessage>()
            .ForCtorParam(nameof(ChatMessage.Guid), x => x.MapFrom(y => Guid.NewGuid()));
    }
}