using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Resenas.Middleware.Rabbit
{
    public class Rabbit
    {
        public Rabbit()
        {

            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // Cambia esto por la dirección de tu servidor RabbitMQ si es necesario
                UserName = "guest", // Cambia esto si usas credenciales diferentes
                Password = "guest" // Cambia esto si usas credenciales diferentes
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "auth", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "auth",
                                  routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    //aca tengo que eliminar el token de redis
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Presiona [enter] para salir.");
                Console.ReadLine();
            }
        }
    }
}
