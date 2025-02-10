using Confluent.Kafka;
using System.Text.Json;

public class MessageProducer<T>
{
    private readonly IProducer<string,string> _producer;
    private readonly string _topic;

    public MessageProducer(IntegrationEvent configs){
       var config = new ProducerConfig { BootstrapServers = configs.BootstrapServers };
       _producer = new ProducerBuilder<string,string>(config).Build();
       _topic = configs.Topic;
    }

    public async Task SendMessageAsync(T message, string? topic = null){
      var targetTopic = topic ?? _topic; 
      var jsonMessage = JsonSerializer.Serialize(message);
      var brokerMessage = new Message<string,string>{Key = Guid.NewGuid().ToString(), Value = jsonMessage};
      await  _producer.ProduceAsync(targetTopic,brokerMessage);
    }

  public void Dispose()=> _producer.Dispose();

}

//Example Code:
//var settings = new IntegrationEvent {Topic = ""};
//var producer = new MessageProducer<ObjetoGenerico>(settings);
//await producer.SendMessageAsync(ObjetoGenerico);