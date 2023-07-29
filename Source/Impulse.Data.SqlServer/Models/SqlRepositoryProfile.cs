using AutoMapper;

namespace Impulse.Data.SqlServer.Models;

internal class SqlRepositoryProfile : Profile
{
    public SqlRepositoryProfile()
    {
        CreateMap<ChatRoom, ChatRoomEntity>()
            .ForCtorParam(nameof(ChatRoomEntity.Id), x => x.MapFrom(y => 0))
            .ReverseMap();

        CreateMap<ChatUser, ChatUserEntity>()
            .ForCtorParam(nameof(ChatUserEntity.Id), x => x.MapFrom(y => 0))
            .ReverseMap();
    }
}