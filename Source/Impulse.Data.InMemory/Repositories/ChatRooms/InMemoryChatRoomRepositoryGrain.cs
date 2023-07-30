using Impulse.Core;
using Impulse.Core.Exceptions;
using Impulse.Core.Extensions;
using System.Collections.Immutable;

namespace Impulse.Data.InMemory.Repositories.ChatRooms;

internal class InMemoryChatRoomRepositoryGrain : Grain, IInMemoryChatRoomRepositoryGrain
{
    private readonly Dictionary<Guid, ChatRoom> _guidIndex = new();
    private readonly Dictionary<string, ChatRoom> _nameIndex = new();

    public Task<IEnumerable<ChatRoom>> GetAll()
    {
        return _guidIndex.Values.ToImmutableArray().AsTaskResult<IEnumerable<ChatRoom>>();
    }

    private void Index(ChatRoom item)
    {
        _guidIndex[item.Guid] = item;
        _nameIndex[item.Name] = item;
    }

    private void Unindex(ChatRoom item)
    {
        _guidIndex.Remove(item.Guid);
        _nameIndex.Remove(item.Name);
    }

    public Task<ChatRoom> Save(ChatRoom item)
    {
        Guard.IsNotNull(item);

        // add new item
        if (!_guidIndex.TryGetValue(item.Guid, out var stored))
        {
            // check name index
            if (_nameIndex.ContainsKey(item.Name))
            {
                throw new InvalidOperationException("Duplicate Name");
            }

            var added = new ChatRoom(item.Guid, item.Name, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, Guid.NewGuid());

            Index(added);

            return added.AsTaskResult();
        }

        // ensure etags match
        if (stored.ETag != item.ETag)
        {
            throw new ConflictException(item.ETag, stored.ETag);
        }

        // ensure no other room has the same name
        if (_nameIndex.TryGetValue(item.Name, out var other) && other.Guid != item.Guid)
        {
            throw new InvalidOperationException("Duplicate Name");
        }

        // save the item
        var saved = new ChatRoom(item.Guid, item.Name, stored.Created, DateTimeOffset.UtcNow, Guid.NewGuid());

        Index(saved);

        return saved.AsTaskResult();
    }

    public Task<ChatRoom?> TryGetByGuid(Guid guid)
    {
        var result = _guidIndex.TryGetValue(guid, out var item) ? item : null;

        return result.AsTaskResult();
    }

    public Task<ChatRoom?> TryGetByName(string name)
    {
        Guard.IsNotNull(name);

        var result = _nameIndex.TryGetValue(name, out var item) ? item : null;

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
}