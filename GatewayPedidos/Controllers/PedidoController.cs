using System.Text;
using System.Text.Json;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace GatewayPedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private IConfiguration _configuration;

        public PedidoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult PostPedidoAsync()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetSection("RabbitMQ").GetValue<string>("Hostname"),
                UserName = _configuration.GetSection("RabbitMQ").GetValue<string>("Username"),
                Password = _configuration.GetSection("RabbitMQ").GetValue<string>("Password")
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

                    var message = JsonSerializer.Serialize(new Pedido
                    {
                        Id = Guid.NewGuid(),
                        Usuario = new Usuario(Guid.NewGuid(), "Teste", "jbrunomf@outlook.com")
                    });

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "fila",
                        basicProperties: null,
                        body: body);
                }
            }

            return Ok();
        }
    }
}
