using src;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

// Fetch the credentials from appsettings.json
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var options = new TwitchOptions();
config.GetSection(TwitchOptions.Twitch).Bind(options);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Create the TwitchAPI client
TwitchAPI twitchApi = new TwitchAPI
{
    Settings = { ClientId = options.ClientId, Secret = options.ClientSecret }
};

// Get the credentials
ConnectionCredentials credentials = new ConnectionCredentials(
    options.Username,
    options.AccessToken
);

var clientOptions = new ClientOptions
{
    //ClientType = ClientType.Chat,
    MessagesAllowedInPeriod = 2000,
    ThrottlingPeriod = TimeSpan.FromSeconds(30)
};
// Create and initialize the TwitchClient
WebSocketClient customClient = new WebSocketClient(clientOptions);
TwitchClient twitchClient = new TwitchClient(customClient);
twitchClient.Initialize(credentials, options.Username);

// Register the services
builder.Services.AddSingleton(twitchApi);
builder.Services.AddSingleton(twitchClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();