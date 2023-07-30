using Impulse.Core;
using Impulse.Data;

namespace Impulse.Grains;

internal class ChatRoomsIndexGrain : Grain, IChatRoomsIndexGrain
{
    public ChatRoomsIndexGrain(IChatRoomRepository repository)
    {
        _repository = repository;
    }

    private readonly IChatRoomRepository _repository;

    /// <summary>
    /// Caches chat rooms indexed by guid.
    /// </summary>
    private readonly Dictionary<Guid, ChatRoom> _guidIndex = new();

    /// <summary>
    /// Caches chat rooms indexed by name.
    /// </summary>
    private readonly Dictionary<string, ChatRoom> _nameIndex = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Indexes the specified chat room.
    /// </summary>
    private void Index(ChatRoom item)
    {
        _guidIndex[item.Guid] = item;
        _nameIndex[item.Name] = item;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var rooms = await _repository.GetAll(cancellationToken);

        foreach (var room in rooms)
        {
            Index(room);
        }
    }

    /// <summary>
    /// Gets the chat room with specified name or adds a new one if not found.
    /// </summary>
    public async Task<ChatRoom> GetOrAdd(string name)
    {
        Guard.IsNotNull(name);

        if (!_nameIndex.TryGetValue(name, out var user))
        {
            user = await _repository.Save(new ChatRoom(Guid.NewGuid(), name));

            Index(user);
        }

        return user;
    }

    /// <summary>
    /// Gets the chat room with specified guid or null if not found.
    /// </summary>
    public Task<ChatRoom?> TryGetByGuid(Guid guid)
    {
        var result = _guidIndex.TryGetValue(guid, out var item) ? item : null;

        return Task.FromResult(result);
    }
}