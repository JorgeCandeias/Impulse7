using Impulse.Data;
using Impulse.Models;
using Impulse.Models.Exceptions;

namespace Impulse.Grains;

internal class ChatUsersIndexGrain : Grain, IChatUsersIndexGrain
{
    public ChatUsersIndexGrain(IChatUserRepository repository)
    {
        _repository = repository;
    }

    private readonly IChatUserRepository _repository;

    /// <summary>
    /// Caches chat users indexed by guid.
    /// </summary>
    private readonly Dictionary<Guid, ChatUser> _guidIndex = new();

    /// <summary>
    /// Caches chat users indexed by name.
    /// </summary>
    private readonly Dictionary<string, ChatUser> _nameIndex = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Indexes the specified user.
    /// </summary>
    private void Index(ChatUser item)
    {
        _guidIndex[item.Guid] = item;
        _nameIndex[item.Name] = item;
    }

    /// <summary>
    /// Unindexes the specified user.
    /// </summary>
    private void Unindex(ChatUser item)
    {
        _guidIndex.Remove(item.Guid);
        _nameIndex.Remove(item.Name);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var users = await _repository.GetAll(cancellationToken);

        foreach (var user in users)
        {
            Index(user);
        }
    }

    /// <summary>
    /// Gets the user with specified name or adds a new one if not found.
    /// </summary>
    public async Task<ChatUser> GetOrAdd(string name)
    {
        Guard.IsNotNull(name);

        if (!_nameIndex.TryGetValue(name, out var user))
        {
            user = await _repository.Save(new ChatUser(Guid.NewGuid(), name));

            Index(user);
        }

        return user;
    }

    /// <summary>
    /// Gets the user with specified guid or null if not found.
    /// </summary>
    public Task<ChatUser?> TryGetByGuid(Guid guid)
    {
        var result = _guidIndex.TryGetValue(guid, out var item) ? item : null;

        return Task.FromResult(result);
    }

    public Task<ImmutableArray<ChatUser>> GetAll()
    {
        var result = _guidIndex.Values.ToImmutableArray();

        return Task.FromResult(result);
    }

    public async Task Remove(Guid guid, Guid etag)
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

        try
        {
            // we can safely remove now
            await _repository.Remove(guid, etag);
        }
        catch (ConflictException)
        {
            // if removing failed then deactive the grain to allow resync
            DeactivateOnIdle();
            throw;
        }

        // if the above succeeded then unindex the item
        Unindex(stored);
    }
}