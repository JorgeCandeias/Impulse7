using Impulse.Models;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat users.
/// </summary>
public interface IChatUserRepository
{
    /// <summary>
    /// Saves the chat user with the specified details.
    /// </summary>
    public Task<ChatUser> Save(ChatUser item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the stored ETag of the chat user with the specified Guid.
    /// </summary>
    Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the chat user with specified guid or null if none is found.
    /// </summary>
    public Task<ChatUser?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the chat user with specified name or null if none is found.
    /// </summary>
    public Task<ChatUser?> TryGetByName(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all chat users.
    /// </summary>
    public Task<IEnumerable<ChatUser>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the user with the specified guid from the repository.
    /// </summary>
    public Task Remove(Guid guid, Guid etag, CancellationToken cancellationToken = default);
}