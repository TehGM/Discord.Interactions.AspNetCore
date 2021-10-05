using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Discord.Interactions.CommandsHandling;
using TehGM.Discord.Interactions.CommandsHandling.Services;

namespace TehGM.Discord.Interactions.Tests
{
    [TestFixture]
    [Category("Services")]
    public class DiscordInteractionCommandsProviderTests
    {
        private IDiscordInteractionCommandsProvider _provider;

        [SetUp]
        public void SetUp()
        {
            this._provider = new DiscordInteractionCommandsProvider();
        }

        [Test]
        public void CommandsProvider_AddCommand_DoesnNotThrow()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);

            Assert.DoesNotThrow(() => this._provider.AddCommand(1234, testCase));
        }

        [Test]
        public void CommandsProvider_AddCommand_CanGet()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);

            this._provider.AddCommand(1234, testCase);
            IDiscordInteractionCommand result = this._provider.GetCommand(1234);

            Assert.IsNotNull(result);
        }

        [Test]
        public void CommandsProvider_GetNotExistingCommand_Fails()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);
            this._provider.AddCommand(1234, testCase);

            IDiscordInteractionCommand result = this._provider.GetCommand(4321);

            Assert.IsNull(result);
        }

        [Test]
        public void CommandsProvider_GetCommand_ReturnsCorrectOne()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);
            IDiscordInteractionCommand testCase2 = new TestCommand(4321);
            this._provider.AddCommand(1234, testCase);
            this._provider.AddCommand(4321, testCase2);

            IDiscordInteractionCommand result = this._provider.GetCommand(4321);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is TestCommand);
            Assert.AreEqual(4321, (result as TestCommand).ID);
        }

        [Test]
        public void CommandsProvider_RemoveCommand_CannotGetRemoved()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);
            this._provider.AddCommand(1234, testCase);

            this._provider.RemoveCommand(1234);
            IDiscordInteractionCommand result = this._provider.GetCommand(1234);

            Assert.IsNull(result);
        }

        [Test]
        public void CommandsProvider_RemoveCommand_RemovesOnlySpecified()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);
            IDiscordInteractionCommand testCase2 = new TestCommand(4321);
            this._provider.AddCommand(1234, testCase);
            this._provider.AddCommand(4321, testCase2);

            this._provider.RemoveCommand(1234);
            IDiscordInteractionCommand result = this._provider.GetCommand(1234);
            IDiscordInteractionCommand result2 = this._provider.GetCommand(4321);

            Assert.IsNull(result);
            Assert.IsNotNull(result2);
        }

        [Test]
        public void CommandsProvider_Clear_RemovesAll()
        {
            IDiscordInteractionCommand testCase = new TestCommand(1234);
            IDiscordInteractionCommand testCase2 = new TestCommand(4321);
            this._provider.AddCommand(1234, testCase);
            this._provider.AddCommand(4321, testCase2);

            this._provider.Clear();
            IDiscordInteractionCommand result = this._provider.GetCommand(1234);
            IDiscordInteractionCommand result2 = this._provider.GetCommand(4321);

            Assert.IsNull(result);
            Assert.IsNull(result2);
        }

        public class TestCommand : IDiscordInteractionCommand
        {
            public ulong ID { get; }

            public TestCommand(ulong id)
                => this.ID = id;

            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken) => Task.FromResult<DiscordInteractionResponse>(null);
        }
    }
}
