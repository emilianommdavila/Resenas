using Newtonsoft.Json;
using Resenas.Middleware.Auth;
using Resenas.Security.Tokens;//Eliminar, esta nada mas para pruebas


namespace Resenas.Middleware.Auth
{
    public class VerificarToken
    {
        private readonly HttpClient _httpClient;
        private readonly string _securityServerUrl;

        public VerificarToken(string securityServerUrl)
        {
            _httpClient = new HttpClient();
            _securityServerUrl = securityServerUrl;
        }
        public async Task<User> obtenerUsuario(string token)
        {

            Tokens.hola();
            try
            {
                var requestUri = $"{_securityServerUrl}/v1/users/current";
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode(); // Throws if not successful

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody); // Imprime el JSON para verificar su formato
                Console.WriteLine(User.FromJson(responseBody)+"fasf"); // Imprime el JSON para verificar su formato
                User hola = User.FromJson(responseBody);
                Console.WriteLine(hola.Login);
                // Deserializar el JSON y devolver el resultado


                return User.FromJson(responseBody);
            }
            catch (HttpRequestException)
            {
                // Handle exception
                return null;
            }
        }

    }
}
