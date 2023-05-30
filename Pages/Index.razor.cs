using Microsoft.AspNetCore.Components;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;

namespace src.Pages;

public partial class Index
{
    TwitchOptions _options = new();
    private User? currentChannel;
    private bool _channelJoined;

    // Get the code parameter from the query
    [Parameter]
    [SupplyParameterFromQuery(Name = "code")]
    public string? Code { get; set; }

    // Inject the dependencies
    [Inject]
    private TwitchAPI _twitchApi { get; set; }

    [Inject]
    private TwitchClient _twitchClient { get; set; }

    [Inject]
    private NavigationManager _navigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        config.GetSection(TwitchOptions.Twitch).Bind(_options);

        if (!string.IsNullOrEmpty(Code))
        {
            var authCoe = Code;
            _navigationManager.NavigateTo("/");
            var authTokenResponse = await _twitchApi.Auth.GetAccessTokenFromCodeAsync(
                authCoe,
                _options.ClientSecret,
                _options.RedirectUri
            );

            var user = await _twitchApi.Helix.Users.GetUsersAsync(
                accessToken: authTokenResponse.AccessToken
            );
            currentChannel = user.Users.First();
        }
        await base.OnInitializedAsync();
        StateHasChanged();
    }

    private void BeginAuthorization()
    {
        var authUrl = Helpers.GetAuthorizationCodeUrl(
            _options.ClientId,
            _options.RedirectUri,
            Helpers.GetScopes()
        );
        _navigationManager.NavigateTo(authUrl);
    }

    private void JoinChannelAsync()
    {
        if (currentChannel is not null)
        {
            var channel = new TwitchChannel
            {
                Id = currentChannel.Id,
                Name = currentChannel.Login,
                Added = DateTimeOffset.Now
            };
            _twitchClient.JoinChannel(channel.Name);
        }
    }

    private void LeaveChannelAsync()
    {
        if (currentChannel is not null)
        {
            var channel = new TwitchChannel
            {
                Id = currentChannel.Id,
                //Name = currentChannel.Login,
                //Added = DateTimeOffset.Now
            };
            _twitchClient.LeaveChannel(currentChannel.Login);
            StateHasChanged();
        }
    }
}
