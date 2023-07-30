using Impulse.Core;
using Impulse.Core.Exceptions;
using Impulse.Core.Extensions;
using System.Collections.Immutable;

namespace Impulse.Data.InMemory.Repositories.ChatMessages;

internal class InMemoryChatMessageRepositoryGrain : Grain, IInMemoryChatMessageRepositoryGrain
{
    private readonly Dictionary<Guid, ChatMessage> _guidIndex = new();
    private readonly Dictionary<string, HashSet<Guid>> _roomIndex = new();

    public Task<IEnumerable<ChatMessage>> GetAll()
    {
        return _guidIndex.Values.ToImmutableArray().AsTaskResult<IEnumerable<ChatMessage>>();
    }

    private void Index(ChatMessage item)
    {
        _guidIndex[item.Guid] = item;

        if (!_roomIndex.TryGetValue(item.Room, out var values))
        {
            values = new();
        }
        values.Add(item.Guid);
    }

    private void Unindex(ChatMessage item)
    {
        _guidIndex.Remove(item.Guid);

        if (_roomIndex.TryGetValue(item.Room, out var values))
        {
            values.Remove(item.Guid);
        }
    }

    public Task<ChatMessage> Save(ChatMessage item)
    {
        Guard.IsNotNull(item);

        // add new item
        if (!_guidIndex.TryGetValue(item.Guid, out var stored))
        {
            var added = new ChatMessage(item.Guid, item.Room, item.User, item.Text, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, Guid.NewGuid());

            Index(added);

            return added.AsTaskResult();
        }

        // ensure etags match
        if (stored.ETag != item.ETag)
        {
            throw new ConflictException(item.ETag, stored.ETag);
        }

        // save the item
        var saved = new ChatMessage(item.Guid, item.Room, item.User, item.Text, stored.Created, DateTimeOffset.UtcNow, Guid.NewGuid());

        Index(saved);

        return saved.AsTaskResult();
    }

    public Task<ChatMessage?> TryGetByGuid(Guid guid)
    {
        var result = _guidIndex.TryGetValue(guid, out var item) ? item : null;

        return result.AsTaskResult();
    }

    public Task<Guid?> TryGetETagByGuid(Guid guid)
    {
        var result = _guidIndex.TryGetValue(guid, out var item) ? item.ETag : (Guid?)null;

        return result.AsTaskResult();
    }

    public Task Remove(Guid guid, Guid etag)
    {
        // throw on non-existing item to remove
        if (!_guidIndex.TryGetValue(guid, out var stored))
        {
            throw new ConflictException(etag, null);
        }

        // throw on wrong etag to remove
        if (etag != stored.ETag)
        {
            throw new ConflictException(etag, stored.ETag);
        }

        // if the above succeeded then unindex the item
        Unindex(stored);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<ChatMessage>> GetLatestCreatedByRoom(string room, int count)
    {
        Guard.IsNotNull(room);
        Guard.IsGreaterThanOrEqualTo(count, 0);

        var result = ImmutableArray.CreateBuilder<ChatMessage>();

        if (_roomIndex.TryGetValue(room, out var values))
        {
            foreach (var guid in values)
            {
                if (_guidIndex.TryGetValue(guid, out var message))
                {
                    result.Add(message);
                }
            }
        }

        return result.ToImmutable().AsTaskResult<IEnumerable<ChatMessage>>();
    }
}