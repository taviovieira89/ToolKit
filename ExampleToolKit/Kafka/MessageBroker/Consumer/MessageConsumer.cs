using Confluent.Kafka;

public abstract class MessageConsumer<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;

    public MessageConsumer(IntegrationEvent integrationEvent)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = integrationEvent.BootstrapServers,
            GroupId = integrationEvent.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<TKey, TValue>(config).Build();
        _consumer.Subscribe(integrationEvent.Topic);
    }

    public void Consume(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                //handleMessage(consumeResult.Message.Key, consumeResult.Message.Value);
                HandleMessage(consumeResult.Message.Key, consumeResult.Message.Value).Wait(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Cancellation requested, exit loop
        }
        finally
        {
            _consumer.Close();
        }
    }

    protected abstract Task HandleMessage(TKey key, TValue value);

    public void Dispose()
    {
        _consumer.Dispose();
    }
}

//Example Code:
//var settings = new IntegrationEvent {Topic = ""};
//var consumer = new MessageConsumer<"","">(settings);
//var cts = new CancellationTokenSource();
//resultConsumer.Consume(cts.Token);