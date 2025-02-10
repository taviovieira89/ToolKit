public class ResultConsumer(IntegrationEvent integrationEvent) : MessageConsumer<string, string>(integrationEvent)
{
    protected override Task HandleMessage(string key, string value)
    {
        Console.WriteLine($"Received message: Key = {key}, Value = {value}");
        return Task.CompletedTask;
    }
}
