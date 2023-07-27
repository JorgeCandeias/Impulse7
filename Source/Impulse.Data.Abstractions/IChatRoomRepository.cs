using Impulse.Core;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat rooms.
/// </summary>
public interface IChatRoomRepository
{
    /// <summary>
    /// Gets or adds a chat room with the specified name.
    /// </summary>
    public Task<ChatRoom> GetOrAdd(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all chat rooms.
    /// </summary>
    public Task<IEnumerable<ChatRoom>> GetAll(CancellationToken cancellationToken = default);
}
