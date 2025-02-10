public class ResultProducer<T> : MessageProducer<T> where T : class
{
    public ResultProducer(IntegrationEvent integrationEvent) : base(integrationEvent)
    {
    }
}