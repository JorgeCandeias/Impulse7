using Impulse.Core;
using Impulse.Core.Exceptions;
using Impulse.Core.Extensions;
using Orleans.Storage;
using System.Collections.Immutable;

namespace Impulse.Data.InMemory.Repositories.ChatUsers;

internal class InMemoryChatUserRepositoryGrain : Grain, IInMemoryChatUserRepositoryGrain
{
    private readonly Dictionary<Guid, ChatUser> _guidIndex = new();
    private readonly Dictionary<string, ChatUser> _nameIndex = new();

    private void Index(ChatUser item)
    {
        _guidIndex[item.Guid] = item;
        _nameIndex[item.Name] = item;
    }

    private void Unindex(ChatUser item)
    {
        _guidIndex.Remove(item.Guid);
        _nameIndex.Remove(item.Name);
    }

    public Task<IEnumerable<ChatUser>> GetAll()
    {
        return _guidIndex.Values.ToImmutableArray().AsTaskResult<IEnumerable<ChatUser>>();
    }

    public Task<ChatUser> Save(ChatUser user)
    {
        Guard.IsNotNull(user);

        // add new item
        if (!_guidIndex.TryGetValue(user.Guid, out var existing))
        {
            // check name index
            if (_nameIndex.ContainsKey(user.Name))
            {
                throw new InvalidOperationException("Duplicate Name");
            }

            var added = new ChatUser(user.Guid, user.Name, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, Guid.NewGuid());

            Index(added);

            return added.AsTaskResult();
        }

        // ensure etags match
        if (existing.ETag != user.ETag)
        {
            throw new InconsistentStateException("Invalid ETag", existing.ETag.ToString(), user.ETag.ToString());
        }

        // ensure no other room has the same name
        if (_nameIndex.TryGetValue(user.Name, out var other) && other.Guid != user.Guid)
        {
            throw new InvalidOperationException("Duplicate Name");
        }

        // save the item
        var saved = new ChatUser(user.Guid, user.Name, existing.Created, DateTimeOffset.UtcNow, Guid.NewGuid());
        
        Index(saved);

        return saved.AsTaskResult();
    }

    public Task<ChatUser?> TryGetByGuid(Guid guid)
    {
        var result = _guidIndex.TryGetValue(guid, out var item) ? item : null;

        return result.AsTaskResult();
    }

    public Task<ChatUser?> TryGetByName(string name)
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