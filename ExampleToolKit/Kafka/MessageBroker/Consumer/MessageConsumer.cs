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

    public async Task<IntegrationEvent> ConsumeAndReturn(CancellationToken cancellationToken)
    {
        var eventData = new IntegrationEvent(string.Empty, new EventData());

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Consome a mensagem do Kafka
                var consumeResult = _consumer.Consume(cancellationToken);

                // Processa a mensagem
                await HandleMessage(consumeResult.Message.Key, consumeResult.Message.Value);

                // Preenche o objeto eventData com a chave e o valor da mensagem
                eventData.Key = consumeResult.Message.Key.ToString();
                eventData.Value = new EventData() { Value = consumeResult.Message.Value.ToString() };
            }

            // Retorna a chave e o evento após consumir a mensagem
            return eventData;
        }
        catch (OperationCanceledException)
        {
            // Caso o processo seja cancelado, retorna valores nulos
            return default!;
        }
        finally
        {
            _consumer.Close(); // Garante que o consumidor seja fechado ao fim
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