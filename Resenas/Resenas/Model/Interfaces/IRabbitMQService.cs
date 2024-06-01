namespace Resenas.Model.Interfaces
{
    public interface IRabbitMQService
    {
        void Publish(string message, string queueName);
        void Consume(string queueName);
    }

}
