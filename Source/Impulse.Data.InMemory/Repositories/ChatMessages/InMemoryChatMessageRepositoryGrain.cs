using Impulse.Core;
using Impulse.Core.Exceptions;
using Impulse.Core.Extensions;
using System.Collections.Immutable;

namespace Impulse.Data.InMemory.Repositories.ChatMessages;

internal class InMemoryChatMessageRepositoryGrain : Grain, IInMemoryChatMessageRepositoryGrain
{
    /// <summary>
    /// Indexes messages by guid.
    /// </summary>
    private readonly Dictionary<Guid, ChatMessage> _guidIndex = new();

    /// <summary>
    /// Indexes messages by room and keeps them in descending order of created date.
    /// </summary>
    private readonly Dictionary<string, SortedSet<ChatMessage>> _roomIndex = new();

    /// <summary>
    /// Defines a comparer that sorts messages by descending order of created date.
    /// </summary>
    private readonly Comparer<ChatMessage> _descendingCreatedComparer = Comparer<ChatMessage>.Create((x, y) =>
    {
        // compare by created date first to ensure intended ordering
        var byCreated = Comparer<DateTimeOffset>.Default.Compare(x.Created, y.Created);
        if (byCreated != 0) return -byCreated;

        // otherwise compare by default record rules
        return Comparer<ChatMessage>.Default.Compare(x, y);
    });

    public Task<IEnumerable<ChatMessage>> GetAll()
    {
        return _guidIndex.Values.ToImmutableArray().AsTaskResult<IEnumerable<ChatMessage>>();
    }

    private void Index(ChatMessage item)
    {
        // index by guid
        _guidIndex[item.Guid] = item;

        // index by room, ordered by descending created date
        if (!_roomIndex.TryGetValue(item.Room, out var values))
        {
            _roomIndex[item.Room] = values = new(_descendingCreatedComparer);
        }
        values.Add(item);
    }

    private void Unindex(ChatMessage item)
    {
        // unindex by guid
        _guidIndex.Remove(item.Guid);

        // unindex by room
        if (_roomIndex.TryGetValue(item.Room, out var values))
        {
            values.Remove(item);

            if (values.Count == 0)
            {
                _roomIndex.Remove(item.Room);
            }
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

        if (count == 0)
        {
            return ImmutableArray<ChatMessage>.Empty.AsTaskResult<IEnumerable<ChatMessage>>();
        }

        var result = ImmutableArray.CreateBuilder<ChatMessage>();

        if (_roomIndex.TryGetValue(room, out var values))
        {
            foreach (var item in values.Take(count))
            {
                result.Add(item);
            }
        }

        return result.ToImmutable().AsTaskResult<IEnumerable<ChatMessage>>();
    }
}