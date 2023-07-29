using Impulse.Core;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat messages.
/// </summary>
public interface IChatMessageRepository
{
    /// <summary>
    /// Saves the specified chat message and returns a copy with any repository updates applied.
    /// </summary>
    Task<ChatMessage> AddMessage(ChatMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest chat messages for the specified room.
    /// </summary>
    Task<IEnumerable<ChatMessage>> GetLatestMessagesByRoom(string room, CancellationToken cancellationToken = default);
}