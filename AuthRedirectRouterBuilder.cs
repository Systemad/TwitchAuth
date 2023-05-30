using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TwitchLib.Api;
using TwitchLib.Client;

namespace src;

public static class AuthRedirectRouterBuilder
{
    public static RouteGroupBuilder MapAuthRedirect(this RouteGroupBuilder routeBuilder)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var options = new TwitchOptions();
        config.GetSection(TwitchOptions.Twitch).Bind(options);

        routeBuilder.MapGet(
            "/redirect/",
            async (
                HttpRequest request,
                [FromServices] TwitchClient twitchBotClient,
                [FromServices] TwitchAPI twitchApi
            ) =>
            {
                var queryString = request.QueryString;
                var queryDictionary = QueryHelpers.ParseQuery(queryString.Value);
                var context = request.HttpContext;
                if (queryDictionary.TryGetValue("code", out var authCode))
                {
                    var authTokenResponse = await twitchApi.Auth.GetAccessTokenFromCodeAsync(
                        authCode,
                        options.ClientSecret,
                        options.RedirectUri
                    );

                    var user = await twitchApi.Helix.Users.GetUsersAsync(
                        accessToken: authTokenResponse.AccessToken
                    );

                    var channel = new TwitchChannel
                    {
                        Id = user.Users.First().Id,
                        Name = user.Users.First().Login,
                        Added = DateTimeOffset.Now
                    };

                    // Let the Twitch Bot join the channel
                    twitchBotClient.JoinChannel(channel.Name);
                }
                //context.Response.Redirect("/");
            }
        );
        return routeBuilder;
    }
}
