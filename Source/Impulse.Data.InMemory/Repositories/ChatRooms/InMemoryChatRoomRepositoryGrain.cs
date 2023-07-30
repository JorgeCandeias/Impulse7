using Impulse.Core;
using Impulse.Core.Exceptions;
using Impulse.Core.Extensions;
using Orleans.Storage;
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

    public Task<ChatRoom> Save(ChatRoom room)
    {
        Guard.IsNotNull(room);

        // add new item
        if (!_guidIndex.TryGetValue(room.Guid, out var existing))
        {
            // check name index
            if (_nameIndex.ContainsKey(room.Name))
            {
                throw new InvalidOperationException("Duplicate Name");
            }

            var added = new ChatRoom(room.Guid, room.Name, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, Guid.NewGuid());

            Index(added);

            return added.AsTaskResult();
        }

        // ensure etags match
        if (existing.ETag != room.ETag)
        {
            throw new InconsistentStateException("Invalid ETag", existing.ETag.ToString(), room.ETag.ToString());
        }

        // ensure no other room has the same name
        if (_nameIndex.TryGetValue(room.Name, out var other) && other.Guid != room.Guid)
        {
            throw new InvalidOperationException("Duplicate Name");
        }

        // save the item
        var saved = new ChatRoom(room.Guid, room.Name, existing.Created, DateTimeOffset.UtcNow, Guid.NewGuid());

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