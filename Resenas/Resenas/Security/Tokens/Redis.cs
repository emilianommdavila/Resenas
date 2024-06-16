using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Resenas.Security.Tokens
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(string redisConnectionString)
        {
            _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _db = _redis.GetDatabase();
        }

       
        public async Task<User> VerificarToken(string token)
        {
            // Obtenemos el valor de la clave
            string valor = await _db.StringGetAsync(token);

            if (!string.IsNullOrEmpty(valor))
            {
                Console.WriteLine("Se encontró el token en redis: " + valor);
                return JsonConvert.DeserializeObject<User>(valor);
            }
            else
            {
                Console.WriteLine("No se encontró el token en redis.");
                return null;
            }
        }

        public void AlmacenarToken(string token, User user)
        {
            // Guardamos un valor en una clave
            string userJson = JsonConvert.SerializeObject(user);
            _db.StringSet(token, userJson);
            Console.WriteLine("Se guardó el token " + token);
        }

        public void EliminarToken(string token)
        {
            // Eliminamos el valor de la clave
            bool eliminado = _db.KeyDelete(token);
            if (eliminado)
            {
                Console.WriteLine("Se eliminó el token: " + token);
            }
            else
            {
                Console.WriteLine("No se encontró el token a eliminar: " + token);
            }
        }

        //internal static async Task<User> verificarToken(string token)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static void almacenarToken(string token, User hola)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
