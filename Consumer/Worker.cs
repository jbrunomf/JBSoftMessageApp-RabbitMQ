using System.Text;
using System.Text.Json;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService

    {
        private readonly ILogger<Worker> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration.GetSection("RabbitMQ").GetValue<string>("Hostname"),
                    UserName = configuration.GetSection("RabbitMq").GetValue<string>("Username"),
                    Password = configuration.GetSection("RabbitMQ").GetValue<string>("Password")
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: "fila",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (sender, args) =>
                        {
                            var body = args.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var pedido = JsonSerializer.Deserialize<Pedido>(message);

                            Console.WriteLine(pedido?.ToString());
                        };

                        channel.BasicConsume(
                            queue: "fila",
                            autoAck: true,
                            consumer: consumer);
                    }

                    //if (_logger.IsEnabled(LogLevel.Information))
                    //{
                    //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    //}

                    await Task.Delay(2000, stoppingToken);
                }
            }

        }
    }
}
