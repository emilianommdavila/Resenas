using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Resenas.Security.Tokens.Redis;
namespace Resenas.Security.Tokens
{
    public class Tokens
    {
      
        public static string hola() {
            //var redisDB = RedisDB.Connection.GetDatabase();
            //redisDB.StringSet("token", "valorDelToken");
            Redis.conectar();
            //var valor = redisDB.StringGet("token");
            //Console.WriteLine(valor);
            return "hola";
        }
    }
}
