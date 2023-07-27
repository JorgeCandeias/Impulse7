using Impulse.Core;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat users.
/// </summary>
public interface IChatUserRepository
{
    /// <summary>
    /// Gets or adds a chat user with the specified name.
    /// </summary>
    public Task<ChatUser> GetOrAdd(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all chat users.
    /// </summary>
    public Task<IEnumerable<ChatUser>> GetAll(CancellationToken cancellationToken = default);
}