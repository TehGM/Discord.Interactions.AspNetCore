# Discord.Interactions.AspNetCore
[![Nuget](https://img.shields.io/nuget/v/TehGM.Discord.Interactions.AspNetCore)](https://www.nuget.org/packages/TehGM.Discord.Interactions.AspNetCore/) [![GitHub top language](https://img.shields.io/github/languages/top/TehGM/Discord.Interactions.AspNetCore)](https://github.com/TehGM/Discord.Interactions.AspNetCore) [![GitHub](https://img.shields.io/github/license/TehGM/Discord.Interactions.AspNetCore)](LICENSE) [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TehGM/Discord.Interactions.AspNetCore/.NET%20Core%20Build)](https://github.com/TehGM/Discord.Interactions.AspNetCore/actions) [![GitHub issues](https://img.shields.io/github/issues/TehGM/Discord.Interactions.AspNetCore)](https://github.com/TehGM/Discord.Interactions.AspNetCore/issues)

This is a simple library designed for ASP.NET Core which helps with enabling interactions (slash/application commands) in ASP.NET Core applications.

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

## Interaction Commands
This library provides an system allowing you design your commands easily. These commands are called Interaction Commands.

The commands, or rather their handlers, are classes that implement [IDiscordInteractionCommandHandler](Discord.Interactions.AspNetCore/CommandsHandling/IDiscordInteractionCommandHandler.cs) interface. [IDiscordInteractionCommandHandler](Discord.Interactions.AspNetCore/CommandsHandling/IDiscordInteractionCommandHandler.cs) requires only one method, `InvokeAsync`, which will be called whenever your application receives the matching interaction command. You should return your response here, which will be sent back to Discord.  
Note that implementation of this method should be lightweight, as Discord server will cancel the interaction after 3 seconds.

```csharp
public class PingCommandHandler : IDiscordInteractionCommandHandler
{
    public async Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, CancellationToken cancellationToken)
    {
        DiscordUser user = message.User ?? message.GuildMember.User;
        return new DiscordInteractionResponseBuilder()
            .WithText($"Pong! {DiscordFormatter.MentionUser(user.ID)}")
            .Build();
    }
}
```

#### Dependency Injection
Command handlers fully support dependency injection via constructor, like other ASP.NET Core services. Handlers with [scoped lifetime](#handler-lifetime) will receive services scoped to the interaction's request.

```csharp
public class PingCommandHandler : IDiscordInteractionCommandHandler
{
    private readonly ILogger _log;

    public PingCommandHandler(ILogger<PingCOmmandHandler> log)
    {
        this._log = log;
    }

    // ... other code such as InvokeAsync here ...
}
```

#### Handler Lifetime
By default, every command handler has scoped lifetime, which should be perfect for most use cases. If you need to change the lifetime of your handler, you can do it with [\[InteractionCommandLifetime\]](Discord.Interactions.AspNetCore/CommandsHandling/InteractionCommandLifetimeAttribute.cs) attribute.

```csharp
[InteractionCommandLifetime(ServiceLifetime.Singleton)]
public class PingCommandHandler : IDiscordInteractionCommandHandler
{
    // ... other code such as InvokeAsync here ...
}
```

#### Disposable Handlers
If your handler implements `IDisposable`, its `Dispose()` method will be called by [DiscordInteractionCommandHandlerCache](Discord.Interactions.AspNetCore/CommandsHandling/Services/Implementations/DiscordInteractionCommandHandlerCache.cs) when it's being disposed by the host - at the end of the request scope for scoped and transient handlers, during application shutdown for singleton ones.

### Using existing Application Commands
If you want to re-use commands you registered previously, you can simply add them to [IDiscordInteractionCommandHandlerFactory](Discord.Interactions.AspNetCore/CommandsHandling/Services/IDiscordInteractionCommandHandlerFactory.cs). You can do it by, for example, using [IHostedService](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice).

In order to add the command, you need to know provide its Discord-assigned ID. You can request it from Discord servers, load from file, hardcode it - your choice, but if the command ID does not match, the command handler will never be executed.

```csharp
// IHostedService code
public class RegisterMyCommands : BackgroundService, IHostedService
{
    private readonly IDiscordInteractionCommandHandlerFactory _handlerFactory;

    public RegisterMyCommands(IDiscordInteractionCommandHandlerFactory handlerFactory)
    {
        this._handlerFactory = handlerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ... load your command IDs
        ulong pingCommandID = // ...

        // add each command
        this._handlerFactory.AddScopedCommand<PingCommandHandler>(pingCommandID);
    }
}
```

Remember to add your service to Startup.cs:
```csharp
// Startup.cs ConfigureServices
services.AddHostedService<RegisterMyCommands>();
```

### Registering new Application Commands
The library will not load and register Application Commands by default. This is opt-in, as enabling this feature will overwrite all commands your application might've already registered with Discord.

However, re-registering commands from the application can be useful if you only run a single instance of the application, as it'll ensure that all your commands are automatically updated and tracked. If you wish to enable this feature, set `RegisterCommands` option to true. Additionally, you'll also need to provide application ID and either [bearer token](https://discord.com/developers/docs/topics/oauth2#client-credentials-grant) or bot token. Both can be found on [Discord Developer Portal](https://discord.com/developers/applications).
```csharp
services.AddDiscordInteractions(options =>
{
    // opt-in to commands registration
    options.RegisterCommands = true;
    // these are required for registering commands
    options.BearerToken = "Discord Issued Bearer Token";
    options.BotToken = "Discord Issued Bot Token";  // bot token is used only if BearerToken is not provided
    options.ApplicationID = "Discord Issued Application ID";
});
```

Additionally, the library will need some information about the commands to be able to register them with Discord. If you only need basic setup, you can use [\[InteractionCommand\]](Discord.Interactions.AspNetCore/CommandsHandling/InteractionCommandAttribute.cs) attribute

```csharp
// example code for slash command /ping
[InteractionCommand("ping", "Pings me!")]
public class PingCommandHandler : IDiscordInteractionCommandHandler
{
    // ... other code such as InvokeAsync here ...
}
```

If you want to register a command that supports options or requires more complex logic, create a new ***static*** method that returns either `DiscordApplicationCommand` or `Task<DiscordApplicationCommand>`, and mark it with [\[InteractionCommandBuilder\]](Discord.Interactions.AspNetCore/CommandsHandling/InteractionCommandBuilderAttribute.cs) attribute. You can inject any non-scoped service into its arguments, as well as `IServiceProvider`and `CancellationToken`.  
Note that [\[InteractionCommand\]](Discord.Interactions.AspNetCore/CommandsHandling/InteractionCommandAttribute.cs) attribute will then be ignored, so you can remove it.

```csharp
// example code for slash command /ping
public class PingCommandHandler : IDiscordInteractionCommandHandler
{
    [InteractionCommandBuilder]
    public static async Task<DiscordApplicationCommand> BuildAsync(ILogger<PingCommandHandler> log, CancellationToken cancellationToken)
    {
        // build your command here
        log.LogDebug("Building command {Command}", "/ping");
        return DiscordApplicationCommandBuilder.CreateSlashCommand("ping", "Pings me!")
            .AddOption(option =>
            {
                // ...
            })
            .Build();
    }
    
    // ... other code such as InvokeAsync here ...
}
```

#### Guild Commands
If you want a command to be added to a specific guild(s), you can use [\[GuildInteractionCommand\]](Discord.Interactions.AspNetCore/CommandsHandling/GuildInteractionCommandAttribute.cs) attribute and provide IDs of the guilds this command should be registered for. Guild Commands will not be registered globally.
```csharp
// example code for slash command /ping
[InteractionCommand("ping", "Pings me!")]
[GuildInteractionCommand(123456789, 987654321)] // register for guilds 123456789 and 987654321
public class PingCommand : IDiscordInteractionCommandHandler
{
    // ... other code such as InvokeAsync here ...
}
```

[\[GuildInteractionCommand\]](Discord.Interactions.AspNetCore/CommandsHandling/GuildInteractionCommandAttribute.cs) attribute works regardless if the command uses [\[InteractionCommand\]](Discord.Interactions.AspNetCore/CommandsHandling/InteractionCommandAttribute.cs) or [\[InteractionCommandBuilder\]](Discord.Interactions.AspNetCore/CommandsHandling/InteractionCommandBuilderAttribute.cs) attribute.

### Adding API Controller
If you want to handle the commands by yourself instead of using the built-in commands handling system, you can add an API Controller on `api/discord/interactions`. Its Post method will be triggered whenever you receive a new interaction that wasn't handled by middlewares.

```csharp
[ApiController]
[Route("api/discord/interactions")]
[AuthorizeDiscordInteraction]   // optional
public class DiscordInteractionsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] DiscordInteraction interaction, CancellationToken cancellationToken)
    {
        // will execute whenever an interaction isn't handled by middlewares
        // interactions are handled by middlewares when they're ping or added commands
        if (interaction.Data.Name == "example")
        {
            // run example command logic
            // use DiscordInteractionResponseBuilder to construct the response, and return it with Ok().
        }
    }
}
```

## Customizing
### Interactions Routes
By default, the middlewares will be configured to only work when a request is made to `/api/discord/interactions`. You can change that when adding Interactions services.
```csharp
services.AddDiscordInteractions(options =>
{
    // remove default route
    options.Routes.Clear();
    // add your custom routes
    // routes need to start with "/", otherwise they won't be recognized by the middleware
    options.Routes.Add("/api/v1/discord/interactions");
});
```
You can add as many routes as you want. Note that it'll run signature verification on each specified route, so do not add routes that will be triggered from different sources than Discord.

> Note: remember to update `[Route]` attribute in your controller.

If you wish, you can also enable the middlewares for all routes in your application by setting `Routes` property to null.
```csharp
services.AddDiscordInteractions(options =>
{
    options.Routes = null;
});
```

#### Route Case Sensitivity
Route matching is case insensitive by default, as `Routes` collection is a [HashSet using `StringComparer.OrdinalIgnoreCase`](Discord.Interactions.AspNetCore/DiscordInteractionsOptions.cs). If you wish to change that, replace entire `Routes` collection.
```csharp
services.AddDiscordInteractions(options =>
{
    // change to case-sensitive invariant culture matching
    options.Routes = new HashSet<string>(StringComparer.InvariantCulture);
    // when overwriting routes collection, routes need to be re-added
    options.Routes.Add("/api/discord/interactions");
});
```

#### Per-route Config
Middlewares support named options, so if you want to for example use different `PublicKey` for each route, you can configure multiple [named options](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0#named-options-support-using-iconfigurenamedoptions) instances, and then enable middlewares using options name.

```csharp
services.Configure<DiscordInteractionsOptions>("App1", options =>
{
    options.PublicKey = "app1_public-key";
    options.Routes.Clear();
    options.Routes.Add("/app1/api/discord/interactions");
});
services.Configure<DiscordInteractionsOptions>("App2", options =>
{
    options.PublicKey = "app2_public-key";
    options.Routes.Clear();
    options.Routes.Add("/app2/api/discord/interactions");
});
```
```csharp
app.UseDiscordInteractions("App1");
app.UseDiscordInteractions("App2");
```

Note that all services, including [command registration services](#registering-new-application-commands), still will use unnamed options.

#### Custom Route Matching
If you want to have full control of which routes the middlewares run on, you need to manually configure them by using ASP.NET Core's [`UseWhen` method](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#branch-the-middleware-pipeline-1). See [DiscordInteractionsMiddlewareExtensions.cs](Discord.Interactions.AspNetCore/DiscordInteractionsMiddlewareExtensions.cs#L34) to see which middlewares you'll need to register manually.

### Automatic Ping Handling
`UseDiscordInteractions` by default automatically registers middleware that will handle [Discord Ping Interactions](https://discord.com/developers/docs/interactions/receiving-and-responding#receiving-an-interaction) for you. If you wish to disable it, set `HandlePings` property to false:
```csharp
services.AddDiscordInteractions(options =>
{
    options.HandlePings = false;
});
```

Note that if you do this, you will need to manually handle these interactions in your controller.

## Development
This library is to be considered as "beta". As such, features might be missing, and breaking changes might be introduced with any update.

Please note that this library is primarily designed for personal use. I do not guarantee full stability, especially if library is used in a way it's not intended to. Feel free to [contribute](#contributing) if needed.

### Known Issues
- Message Components (for example, message buttons) aren't supported currently. Support is planned for future versions.
- [Followup Messages](https://discord.com/developers/docs/interactions/receiving-and-responding#followup-messages) aren't supported yet, but are planned.

### Contributing
In case you want to report a bug or request a new feature, open a new [Issue](https://github.com/TehGM/Discord.Interactions.AspNetCore/issues).

If you want to contribute a patch or update, fork repository, implement the change, and open a pull request.

## License
Copyright (c) 2021 TehGM 

Licensed under [MIT License](LICENSE).