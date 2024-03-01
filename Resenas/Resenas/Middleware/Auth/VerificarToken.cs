using Newtonsoft.Json;

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
            try
            {
                var requestUri = $"{_securityServerUrl}/v1/users/current";
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode(); // Throws if not successful

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(responseBody);
            }
            catch (HttpRequestException)
            {
                // Handle exception
                return null;
            }
        }


    }
}
