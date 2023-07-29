using Impulse.Core;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat rooms.
/// </summary>
public interface IChatRoomRepository
{
    /// <summary>
    /// Saves the chat room with the specified details.
    /// </summary>
    public Task<ChatRoom> Save(ChatRoom room, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the chat room with specified guid or null if none is found.
    /// </summary>
    public Task<ChatRoom?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the chat room with specified name or null if none is found.
    /// </summary>
    public Task<ChatRoom?> TryGetByName(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all chat rooms.
    /// </summary>
    public Task<IEnumerable<ChatRoom>> GetAll(CancellationToken cancellationToken = default);
}