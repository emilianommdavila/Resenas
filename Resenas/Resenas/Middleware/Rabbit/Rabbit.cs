using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Resenas.Model.Interfaces;
using Resenas.Security.Tokens;
using System.Text;

namespace Resenas.Middleware.Rabbit
{
    public class Rabbit : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "auth";
        private readonly IRedisService _redisService;

        public Rabbit(IConnection connection, IRedisService redisService)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _redisService = redisService;
            Console.WriteLine(" Esta andando");
            ConfigureRabbitMQ();
            StartListening();
        }

        private void ConfigureRabbitMQ()
        {
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);
        }

        private void StartListening()
        {
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            Console.WriteLine(" Esta andando");
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                // Aquí debes eliminar el token de Redis
                var logoutMessage = JsonConvert.DeserializeObject<LogoutMessage>(message);
                var token = ExtraerToken(logoutMessage.Message);

                // Eliminar el token de Redis
                _redisService.EliminarToken(token);
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Publish(string message, string queueName)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }

        public void Consume(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                // Aquí puedes procesar el mensaje recibido
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private string ExtraerToken(string message)
        {
            const string bearerPrefix = "bearer ";
            if (message.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return message.Substring(bearerPrefix.Length).Trim();
            }
            return message;
        }
    }
    public class LogoutMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
