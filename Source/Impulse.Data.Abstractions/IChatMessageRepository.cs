using Impulse.Models;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat messages.
/// </summary>
public interface IChatMessageRepository
{
    /// <summary>
    /// Saves the chat message with the specified details.
    /// </summary>
    public Task<ChatMessage> Save(ChatMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the stored ETag of the message with the specified Guid.
    /// </summary>
    Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the chat message with specified guid or null if none is found.
    /// </summary>
    public Task<ChatMessage?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all chat messages.
    /// </summary>
    public Task<IEnumerable<ChatMessage>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest created messages in the specified chat room.
    /// </summary>
    public Task<IEnumerable<ChatMessage>> GetLatestCreatedByRoom(string room, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the message with the specified guid from the repository.
    /// </summary>
    public Task Remove(Guid guid, Guid etag, CancellationToken cancellationToken = default);
}