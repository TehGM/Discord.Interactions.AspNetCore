using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Discord.Interactions.CommandsHandling;
using TehGM.Discord.Interactions.CommandsHandling.Services;

namespace TehGM.Discord.Interactions.Tests
{
    [TestFixture]
    [Category("Services")]
    public class DiscordApplicationCommandsCreatorTests
    {
        private IDiscordApplicationCommandsCreator _creator;

        [SetUp]
        public void SetUp()
        {
            IServiceCollection services = new ServiceCollection();
            this._creator = new DiscordApplicationCommandsCreator(services.BuildServiceProvider());
        }

        [Test]
        public void CommandsCreator_AttributeCommand_Success()
        {
            IDiscordInteractionCommand testCase = new AttributeCommand();

            DiscordApplicationCommand result = this._creator.Create(testCase);

            Assert.IsNotNull(result);
            Assert.AreEqual("attribute_name", result.Name);
            Assert.AreEqual("attribute_description", result.Description);
        }

        [Test]
        public void CommandsCreator_InterfaceCommand_Success()
        {
            IDiscordInteractionCommand testCase = new InterfaceCommand();

            DiscordApplicationCommand result = this._creator.Create(testCase);

            Assert.IsNotNull(result);
            Assert.AreEqual("interface_name", result.Name);
            Assert.AreEqual("interface_description", result.Description);
        }

        [Test]
        public void CommandsCreator_CombinedCommand_InterfaceHasPriority()
        {
            IDiscordInteractionCommand testCase = new CombinedCommand();

            DiscordApplicationCommand result = this._creator.Create(testCase);

            Assert.IsNotNull(result);
            Assert.AreEqual("interface_name", result.Name);
            Assert.AreEqual("interface_description", result.Description);
        }

        [Test]
        public void CommandsCreator_InvalidCommand_Fails()
        {
            IDiscordInteractionCommand testCase = new InvalidCommand();

            ReturnsTestDelegate result = () => this._creator.Create(testCase);

            CustomAssert.CatchOrNull(result);
        }

        [InteractionCommand("attribute_name", "attribute_description")]
        public class AttributeCommand : IDiscordInteractionCommand
        {
            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken) => Task.FromResult<DiscordInteractionResponse>(null);
        }

        public class InterfaceCommand : IDiscordInteractionCommand, IBuildableDiscordInteractionCommand
        {
            public DiscordApplicationCommand Build(IServiceProvider services)
                => new DiscordApplicationCommand(DiscordApplicationCommandType.ChatInput, "interface_name", "interface_description", true);

            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken) => Task.FromResult<DiscordInteractionResponse>(null);
        }

        [InteractionCommand("attribute_name", "attribute_description")]
        public class CombinedCommand : IDiscordInteractionCommand, IBuildableDiscordInteractionCommand
        {
            public DiscordApplicationCommand Build(IServiceProvider services)
                => new DiscordApplicationCommand(DiscordApplicationCommandType.ChatInput, "interface_name", "interface_description", true);

            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken) => Task.FromResult<DiscordInteractionResponse>(null);
        }

        public class InvalidCommand : IDiscordInteractionCommand
        {
            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken) => Task.FromResult<DiscordInteractionResponse>(null);
        }
    }
}
