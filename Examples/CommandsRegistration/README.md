# Commands Registration Example
This example demonstrates how to build and register commands from your application when using ***Discord.Interactions.AspNetCore*** library.

## Contents
- [Startup.cs](Startup.cs) file demonstrates how to configure services and middlewares.
- [appsettings.json](appsettings.json) file shows placeholder configuration.

Command examples:
- [PingCommandHandler.cs](Commands/PingCommandHandler.cs) shows how to declare a simple command using an attribute.
- [SayCommandHandler.cs](Commands/SayCommandHandler.cs) shows how to declare a command with options using a builder method.
- [GuildCommandHandler.cs](Commands/GuildCommandHandler.cs) shows how to create a command for specific guilds only.

## Running the example
Before running the example, you need to provide valid values to the configuration. You can update [appsettings.json](appsettings.json), but the recommended way is to create a git-ignored file. In this project, `appsettings.Development.json` is automatically ignored, so you can create it and store secrets there.

Once the config is updated, simply run the example from Visual Studio.

Interaction commands require that you specify endpoint URL for your application on [Discord Developer Portal](https://discord.com/developers/applications). You should do it while the example application is already running, as Discord will try to validate the endpoint.  
I recommend using [ngrok](https://ngrok.com/) or get URL that will securely tunnel to your application.

When ngrok is installed, and example application is running, open cmd and run `ngrok http 5444`.  
Example endpoint URL would be something like `https://51a4-193-104-143-9.ngrok.io/api/discord/interactions` - which is what you'll need to put into [Discord Developer Portal](https://discord.com/developers/applications).