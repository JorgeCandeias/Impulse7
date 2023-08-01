using Impulse.Models;

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

        CreateMap<ActiveChatRoomAddMessageRequest, ChatMessage>()
            .ForCtorParam(nameof(ChatMessage.Guid), x => x.MapFrom(y => Guid.NewGuid()))
            .ForCtorParam(nameof(ChatMessage.Created), x => x.MapFrom(y => DateTimeOffset.UtcNow))
            .ForCtorParam(nameof(ChatMessage.Updated), x => x.MapFrom(y => DateTimeOffset.UtcNow));

        CreateMap<ActiveChatRoomSiloStats, ActiveChatRoomSiloStatsResponse>()
            .ForCtorParam(nameof(ActiveChatRoomSiloStatsResponse.SiloAddress), x => x.MapFrom(y => y.SiloAddress.ToParsableString()));

        CreateMap<ActiveChatRoomClusterStats, ActiveChatRoomClusterStatsResponse>();
    }
}