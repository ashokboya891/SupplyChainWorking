using RabbitMQ.Client;


namespace SupplyChain.IServiceContracts
{
    public interface IRabbitMQService
    {
        void Publish(string queueName, string message);

    }
}
