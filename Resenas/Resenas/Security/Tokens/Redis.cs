using Newtonsoft.Json;
using StackExchange.Redis;
using System.Drawing;
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
        public async static Task<User> verificarToken(string token) {
            // Conexión a Redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

            // Obtenemos una instancia del servidor Redis
            IDatabase db = redis.GetDatabase();
            // Obtenemos el valor de la clave
            string valor = db.StringGet(token);

            if (valor != null)
            {
                Console.WriteLine("Se encontro el token en redis: " + valor);
            }
            else
            {
                Console.WriteLine("No se encontro el token en redis: " + valor);
                return null;
            }
            redis.Close();
            return JsonConvert.DeserializeObject<User>(valor);
        }

        public static void almacenarToken(string token, User user)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

            // Obtenemos una instancia del servidor Redis
            IDatabase db = redis.GetDatabase();

            // Guardamos un valor en una clave
            string userJson = JsonConvert.SerializeObject(user);
            db.StringSet(token, userJson);
            Console.WriteLine("Se guardo el token " + token);
            redis.Close();
        }
       
    }


}



