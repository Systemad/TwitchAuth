namespace src;

public class TwitchOptions
{
    public const string Twitch = "Twitch";

    public string Username { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class TwitchChannel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset Added { get; set; }
}
