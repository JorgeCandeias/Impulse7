using AutoMapper;
using Impulse.Core;

namespace Impulse.WebApi.Models;

internal class ApiModelsProfile : Profile
{
    public ApiModelsProfile()
    {
        CreateMap<ChatUser, ApiChatUser>();
    }
}