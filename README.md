# Discord.Interactions.AspNetCore
[![Nuget](https://img.shields.io/nuget/v/TehGM.Discord.Interactions.AspNetCore)](https://www.nuget.org/packages/TehGM.Discord.Interactions.AspNetCore/) [![GitHub top language](https://img.shields.io/github/languages/top/TehGM/Discord.Interactions.AspNetCore)](https://github.com/TehGM/Discord.Interactions.AspNetCore) [![GitHub](https://img.shields.io/github/license/TehGM/Discord.Interactions.AspNetCore)](LICENSE) [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TehGM/Discord.Interactions.AspNetCore/.NET%20Core%20Build)](https://github.com/TehGM/Discord.Interactions.AspNetCore/actions) [![GitHub issues](https://img.shields.io/github/issues/TehGM/Discord.Interactions.AspNetCore)](https://github.com/TehGM/Discord.Interactions.AspNetCore/issues)

This is a simple library designed for ASP.NET Core which helps with enabling interactions (slash/application commands) in ASP.NET Core applications.

> Please note that this library is primarily designed for personal use, and therefore might not support all required features. I do not guarantee full stability, especially if library is used in a way it's not intended to - I can however confirm that it works, as I use it for my own projects. Feel free to [contribute](#contributing) if needed.

> If you want to write a fully-fledged Discord bot, please check out [other community libraries](https://discord.com/developers/docs/topics/community-resources#libraries).

## Setting Up

### Getting Public Key
Before starting, you need public key of your Discord application. You can get one on [Discord Developer Portal](https://discord.com/developers/applications).

Once you have it, either add it to your application configuration (see [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0)), or alternatively you can set it directly in code.

### Install the library
Library will be available on nuget.org soon(tm).

### Configure Services
You need to configure a few services in your Startup: configure options, and also add Interactions Services and Interactions Authentication.
```csharp
// load configuration from providers
services.Configure<DiscordInteractionsOptions>(Configuration.GetSection("Discord"));

// add Interactions Services
services.AddDiscordInteractions(options =>
{
    // if you didn't add Discord Application Public Key to configuration, you can do it here
    // options.PublicKey = "foobar";
});

// add authentication
services.AddAuthentication()
    .AddDiscordInteractions();
```

> Tip: Adding authentication is optional, but doing so will enable automatic User claims parsing whenever `[AuthorizeDiscordInteraction]` attribute is used.

### Enabling Middlewares
Next, add Discord Interactions middlewares. They'll do a few automatic steps for you, like required [Signature Verification](https://discord.com/developers/docs/interactions/receiving-and-responding#security-and-authorization), and also will automatically respond to Discord's ping messages.

Add following code to your Configure method in Startup:
```csharp
app.UseDiscordInteractions();
```

### Adding API Controller
Now you can add a new API Controller on `api/discord/interactions` route. Its Post method will be triggered whenever you receive a new non-ping interaction.

```csharp
[ApiController]
[Route("api/discord/interactions")]
[AuthorizeDiscordInteraction]   // optional
public class DiscordInteractionsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] DiscordInteraction interaction, CancellationToken cancellationToken)
    {
        // will execute whenever a new non-ping interaction is received
    }
}
```

## Customizing
### Interactions Routes
By default, the middlewares will be configured to only work when a request is made to `/api/discord/interactions`. You can change that when adding Discord Interactions Middlewares.
```csharp
app.UseDiscordInteractions(options =>
{
    // remove default route
    options.Routes.Clear();
    // add your custom routes
    // routes need to start with "/", otherwise they won't be recognized by the middleware
    options.Routes.Add("/api/v1/discord/interactions");
});
```
You can add as many routes as you want. Note that it'll run signature verification on each specified route.

> Note: remember to update `[Route]` attribute in your controller.

If you wish, you can also enable the middlewares for all routes in your application by setting `Routes` property to null.
```csharp
app.UseDiscordInteractions(options =>
{
    options.Routes = null;
});
```

#### Route Case Sensitivity
Route matching is case insensitive by default, as `Routes` collection is a [HashSet using `StringComparer.OrdinalIgnoreCase`](Discord.Interactions.AspNetCore/Middlewares/DiscordInteractionsMiddlewareOptions.cs). If you wish to change that, replace entire `Routes` collection.
```csharp
app.UseDiscordInteractions(options =>
{
    // change to case-sensitive invariant culture matching
    options.Routes = new HashSet<string>(StringComparer.InvariantCulture);
    // when overwriting routes collection, routes need to be re-added
    options.Routes.Add("/api/discord/interactions");
});
```

#### Custom Route Matching
If you want to have full control of which routes the middlewares run on, you need to manually configure them by using ASP.NET Core's [`UseWhen` method](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#branch-the-middleware-pipeline-1). See [DiscordInteractionsMiddlewareExtensions.cs](Discord.Interactions.AspNetCore/DiscordInteractionsMiddlewareExtensions.cs#L34) to see which middlewares you'll need to register manually.

### Automatic Ping Handling
`UseDiscordInteractions` by default automatically registers middleware that will handle [Discord Ping Interactions](https://discord.com/developers/docs/interactions/receiving-and-responding#receiving-an-interaction) for you. If you wish to disable it, set `HandlePings` property to false:
```csharp
app.UseDiscordInteractions(options =>
{
    options.HandlePings = false;
});
```

Note that if you do this, you will need to manually handle these interactions in your controller.

## Contributing
In case you want to report a bug or request a new feature, open a new [Issue](https://github.com/TehGM/Discord.Interactions.AspNetCore/issues).

If you want to contribute a patch or update, fork repository, implement the change, and open a pull request.

## License
Copyright (c) 2021 TehGM 

Licensed under [MIT License](LICENSE).