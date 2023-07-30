using Impulse.Core;
using Impulse.Data;

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
}