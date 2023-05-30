# Twitch Authorization Blazor Example

This is a very simple and bare bones example on how to add a twitch Bot to a channel using Authorization code grant flow.

## How it works

1. Generates a URL with scope, redirect URL, and authorization code grant flow type
2. User presses a button to navigate to URL
3. User sign in with Twitch Credentials and gets redirected back to Blazor Server page
4. Blazor page extracts the code from the query
5. Uses the TwitchAPI with the help of Twitch Bot Application access token to retrieve the specific user information

The Index.Razor.cs file also has 2 function that demonstrates the bot joining the channel.

This only sets up so the bot has permission to moderate the joined users channel.

Ideally, you'd want to set up a persistence and store the username or the entire TwitchChannel object, then on application start, have a function that iterates over the users and have the TwitchClient joining the channel by username.

```csharp
private TwitchClient _twitchClient = ...

_twitchClient.OnConnected += async (sender, args) => await OnConnectedAsync(sender, args);
```

```csharp
private async Task OnConnectedAsync(object sender, OnConnectedArgs args)
{
    Log.Information("OnConnectedAsync: Connected to {@username}", args.BotUsername);
    await using var context = _contextFactory.CreateDbContext();
    var channels = await context.Channels.ToListAsync();

    foreach (var channel in channels)
    {
        Log.Information("Connecting to {@channel}", channel.Name);
        _twitchClient.JoinChannel(channel.Name);
    }
}
```

## Acknowledgements

- [Twitch Developer](https://dev.twitch.tv/docs/authentication/getting-tokens-oauth)
- [TwitchLib](https://github.com/TwitchLib/TwitchLib)