using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;



/// ////REVISAAAAAAAAAAAAAAAAAAAAAAAR



public interface IUser
{
    string Id { get; set; }
    string Name { get; set; }
    string Login { get; set; }
    string[] Permissions { get; set; }
}

public interface ISession
{
    string Token { get; set; }
    IUser User { get; set; }
}

public static class SessionManager
{
    private static readonly MemoryCache SessionCache = new MemoryCache(new MemoryCacheOptions());
    private static readonly HttpClient RestClient = new HttpClient();

    public static async Task<ISession> Validate(string auth)
    {
        if (SessionCache.TryGetValue(auth, out ISession cachedSession))
        {
            return cachedSession;
        }

        var conf = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); // You may need to adjust this based on your setup

        RestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth);
        var response = await RestClient.GetStringAsync($"{conf}/v1/users/current"); // Adjust the endpoint URL as needed

        var user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(response);
        var session = new Session { Token = auth, User = user };

        SessionCache.Set(auth, session, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            SlidingExpiration = TimeSpan.FromSeconds(60)
        });

        return session;
    }

    public static void Invalidate(string token)
    {
        if (SessionCache.TryGetValue(token, out _))
        {
            SessionCache.Remove(token);
            Console.WriteLine($"RabbitMQ session invalidated {token}");
        }
    }
}



public class Session : ISession
{
    public string Token { get; set; }
    public IUser User { get; set; }
}
