using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using SupplyChain.DTOs;
using SupplyChain.IServiceContracts;
using System.Text;
using System.Text.Json;

namespace SupplyChain.Services
{
    public class RabbitMQService: IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMQService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Publish(string queueName,PaymentNotificationMessage message)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            //var body = Encoding.UTF8.GetBytes(message);
            // Serialize object to JSON
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: null,
                                  body: body);

            //Console.WriteLine($"📤 Published: {message}");
            Console.WriteLine($"📤 Published: {json}");

        }
    }
}
