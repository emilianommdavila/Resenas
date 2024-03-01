using Newtonsoft.Json;

public class User : IUser
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("login")]
    public string Login { get; set; }

    [JsonProperty("permissions")]
    public string[] Permissions { get; set; }

    public static User FromJson(string json)
    {
        return JsonConvert.DeserializeObject<User>(json);
    }

    public static implicit operator User(Task<User> v)
    {
        throw new NotImplementedException();
    }
}
