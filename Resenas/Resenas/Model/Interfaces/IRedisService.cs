using System.Threading.Tasks;

namespace Resenas.Security.Tokens
{
    public interface IRedisService
    {
        Task<User> VerificarToken(string token);
        void AlmacenarToken(string token, User user);
        void EliminarToken(string token);
    }
}
