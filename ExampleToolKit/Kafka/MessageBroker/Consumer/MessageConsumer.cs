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
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Consome a mensagem do Kafka
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult != null && !consumeResult.IsPartitionEOF)
                {
                    // Processa a mensagem
                    await HandleMessage(consumeResult.Message.Key, consumeResult.Message.Value);

                    // Cria e retorna o objeto IntegrationEvent
                    var eventData = new IntegrationEvent(
                        key: consumeResult.Message.Key!.ToString()!,
                        value: new EventData { Value = consumeResult.Message.Value!.ToString()! }
                    );

                    Console.WriteLine($"Mensagem consumida e retornada: Key = {eventData.Key}, Value = {eventData.Value.Value}");
                    return eventData;
                }
                else
                {
                    // Se não houver mensagem, aguarda um pouco antes de tentar novamente
                    Console.WriteLine("Nenhuma mensagem disponível. Aguardando...");
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumo cancelado.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consumir mensagem: {ex.Message}");
        }
        finally
        {
            _consumer.Close(); // Garante que o consumidor seja fechado ao fim
        }

        return null!; // Retorna null se nenhuma mensagem for consumida
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