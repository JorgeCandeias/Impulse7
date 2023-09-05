namespace Impulse.Grains;

internal class ActiveChatRoomTracingGrain : Grain, IActiveChatRoomTracingGrain, IAsyncObserver<ChatMessage>
{
    public ActiveChatRoomTracingGrain(IOptions<ActiveChatRoomTracingOptions> options)
    {
        _options = options.Value;
    }

    private readonly ActiveChatRoomTracingOptions _options;

    private IAsyncStream<ChatMessage> _stream = null!;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var room = this.GetPrimaryKeyString();

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(room);

        // attach the grain instance as the handler of incoming messages
        var handles = await _stream.GetAllSubscriptionHandles();
        foreach (var handle in handles)
        {
            await handle.ResumeAsync(this);
        }

        // optional - remove duplicate subscriptions if added by accident
        foreach (var handle in handles.Skip(1))
        {
            await handle.UnsubscribeAsync();
        }

        // create the output path
        Directory.CreateDirectory(_options.OutputPath);
    }

    public async Task Start()
    {
        // verify existing handles to avoid duplication
        var handles = await _stream.GetAllSubscriptionHandles();
        if (handles.Count > 0)
        {
            return;
        }

        // otherwise subscribe as normal
        await _stream.SubscribeAsync(this);
    }

    public async Task Stop()
    {
        foreach (var handle in await _stream.GetAllSubscriptionHandles())
        {
            await handle.UnsubscribeAsync();
        }
    }

    #region Streaming

    public async Task OnNextAsync(ChatMessage item, StreamSequenceToken? token = null)
    {
        var path = Path.Combine(_options.OutputPath, $"{item.Guid}.json");

        using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        await JsonSerializer.SerializeAsync(file, item);
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

    #endregion Streaming
}