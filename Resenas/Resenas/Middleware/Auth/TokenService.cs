using System.Collections.Concurrent;

namespace Resenas.Middleware.Auth
{
    public class TokenService
    {
        private readonly VerificarToken _verificarToken;
        private readonly ConcurrentDictionary<string, User> _userCache;

        public TokenService(VerificarToken verificarToken)
        {
            _verificarToken = verificarToken;
            _userCache = new ConcurrentDictionary<string, User>();
        }
        public void ValidateAdmin(string token)
        {
            ValidateLoggedIn(token);
            if (!_userCache.TryGetValue(token, out User cachedUser) || cachedUser == null || !Contains(cachedUser.Permissions, "admin"))
            {
                throw new UnauthorizedException();
            }
        }
        public async void ValidateLoggedIn(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedException();
            }

            if (_userCache.TryGetValue(token, out User cachedUser) && cachedUser != null)
            {
                return;
            }

            var user = await _verificarToken.obtenerUsuario(token);
            if (user == null)
            {
                throw new UnauthorizedException();
            }

            _userCache[token] = user;
        }

        public void Invalidate(string token)
        {
            _userCache.TryRemove(token, out _);
        }

        private bool Contains(string[] permissions, string permission)
        {
            foreach (var p in permissions)
            {
                if (p.Equals(permission, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Unauthorized access.") { }
    }

}
