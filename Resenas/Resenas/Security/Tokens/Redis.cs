using StackExchange.Redis;
namespace Resenas.Security.Tokens
{
    public class Redis
    {
        //public class RedisDB
        //{
        //    private static Lazy<ConnectionMultiplexer> _lazyConnection;
        //    public static ConnectionMultiplexer Connection
        //    {
        //        get
        //        {
        //            return _lazyConnection.Value;
        //        }
        //    }
        public static void conectar() {
            // Conexión a Redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

            // Obtenemos una instancia del servidor Redis
            IDatabase db = redis.GetDatabase();

            // Guardamos un valor en una clave
            db.StringSet("clave", "Hola, Redis!");

            // Obtenemos el valor de la clave
            string valor = db.StringGet("clave");

            Console.WriteLine("Valor de la clave 'clave': " + valor);

            // Cerramos la conexión a Redis
            redis.Close();

        }
       
    }


}



