namespace Impulse.Core;

[Immutable]
[GenerateSerializer]
public record ChatMessage
{
    [Id(0)]
    public int Id { get; init; }

    [Id(1)]
    public string User { get; init; } = "";

    [Id(2)]
    public string Text { get; init; } = "";

    [Id(3)]
    public DateTimeOffset Created { get; init; }
}