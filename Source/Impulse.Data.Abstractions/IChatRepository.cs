using Impulse.Core;

namespace Impulse.Data;

/// <summary>
/// Represents a repository that stores chat messages.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// Gets a chat message by ID.
    /// </summary>
    Task<ChatMessage> GetMessage(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new chat message.
    /// </summary>
    Task<ChatMessage> AddMessage(ChatMessage message, CancellationToken cancellationToken = default);
}
