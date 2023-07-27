using AutoMapper;

namespace Impulse.Data.SqlServer.Models;

internal class SqlRepositoryProfile : Profile
{
    public SqlRepositoryProfile()
    {
        CreateMap<ChatRoomEntity, ChatRoom>();

        CreateMap<ChatUserEntity, ChatUser>();
    }
}