using MailKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SupplyChain.DTOs;
using System.Text;
using System.Text.Json;

namespace SupplyChain.BG
{
    public class RabbitMQConsumerService: BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly MailService _mailService;

        public RabbitMQConsumerService(MailService mailService)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "payment-queue", durable: false, exclusive: false, autoDelete: false);

            _mailService = mailService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"📥 Received message: {json}");
                Console.ResetColor();

                // Deserialize the message back to object
                var message = JsonSerializer.Deserialize<PaymentNotificationMessage>(json);

                if (message != null)
                {
                    // Send email to sender
                    await _mailService.SendEmailAsync(
                        message.SenderEmail,
                        "✅ Payment Confirmation",
                        $"Hi {message.SenderUsername}, your payment for Order ID {message.OrderId} was successful at {message.Timestamp}."
                    );

                    // Send email to receiver (fixed merchant)
                    await _mailService.SendEmailAsync(
                        message.ReceiverEmail,
                        "💰 New Payment Received",
                        $"You received a payment of Order ID {message.OrderId} from {message.SenderUsername} at {message.Timestamp}."
                    );
                }
            };


            //consumer.Received += async (model, ea) =>
            //{
            //    var body = ea.Body.ToArray();
            //    var message = Encoding.UTF8.GetString(body);

            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine($"📥 Received message: {message}");
            //    Console.ResetColor();

            //    await _mailService.SendEmailAsync("aboya375@gmail.com", "New Payment Received", message);
            //};

            _channel.BasicConsume(queue: "payment-queue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
        //private IConnection _connection;
        //private RabbitMQ.Client.IModel _channel;

        //public RabbitMQConsumerService()
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    _connection = factory.CreateConnection();
        //    _channel = _connection.CreateModel();

        //    _channel.QueueDeclare(queue: "payment-queue",
        //                          durable: false,
        //                          exclusive: false,
        //                          autoDelete: false,
        //                          arguments: null);
        //}

        //protected override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    //Thread.Sleep(10000); // 10 seconds delay

        //    var consumer = new EventingBasicConsumer(_channel);

        //    consumer.Received += (model, ea) =>
        //    {
        //        var body = ea.Body.ToArray();
        //        var message = Encoding.UTF8.GetString(body);

        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.WriteLine($"📥 Received message from payment-queue: {message}");
        //        Console.ResetColor();

        //        // 👉 Optional: Save to DB or Send Email here
        //    };

        //    _channel.BasicConsume(queue: "payment-queue",
        //                          autoAck: true,
        //                          consumer: consumer);

        //    return Task.CompletedTask;
        //}

        //public override void Dispose()
        //{
        //    _channel.Close();
        //    _connection.Close();
        //    base.Dispose();
        //}
    }
}
