using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SupplyChain.BG
{
    public class RabbitMQLoginConsumerService:BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly MailService _mailService;

        public RabbitMQLoginConsumerService(MailService mailService)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "login-queue", durable: false, exclusive: false, autoDelete: false);

            _mailService = mailService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"📥 Received message: {message}");
                Console.ResetColor();

                await _mailService.SendEmailAsync("aboya375@gmail.com", "🔐 Login Alert", message);
            };

            _channel.BasicConsume(queue: "login-queue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
