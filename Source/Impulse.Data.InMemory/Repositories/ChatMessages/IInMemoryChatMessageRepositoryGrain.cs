using Impulse.Core;

namespace Impulse.Data.InMemory.Repositories.ChatMessages;

internal interface IInMemoryChatMessageRepositoryGrain : IGrainWithStringKey
{
    public Task<IEnumerable<ChatMessage>> GetAll();

    public Task<ChatMessage> Save(ChatMessage room);

    public Task<ChatMessage?> TryGetByGuid(Guid guid);

    public Task<Guid?> TryGetETagByGuid(Guid guid);

    public Task Remove(Guid guid, Guid etag);

    public Task<IEnumerable<ChatMessage>> GetLatestCreatedByRoom(string room, int count);
}

internal static class InMemoryChatMessageRepositoryGrainFactoryExtensions
{
    public static IInMemoryChatMessageRepositoryGrain GetInMemoryChatMessageRepositoryGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IInMemoryChatMessageRepositoryGrain>("DEFAULT");
    }
}