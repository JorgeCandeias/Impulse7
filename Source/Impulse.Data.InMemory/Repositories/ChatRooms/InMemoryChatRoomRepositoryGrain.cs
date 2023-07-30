using Impulse.Core;
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
            _guidIndex[added.Guid] = added;
            _nameIndex[added.Name] = added;

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
        _guidIndex[saved.Guid] = saved;
        _nameIndex[saved.Name] = saved;

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
}