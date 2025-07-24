using RabbitMQ.Client;
using SupplyChain.DTOs;


namespace SupplyChain.IServiceContracts
{
    public interface IRabbitMQService
    {
        void Publish(string queueName, PaymentNotificationMessage message);

    }
}
