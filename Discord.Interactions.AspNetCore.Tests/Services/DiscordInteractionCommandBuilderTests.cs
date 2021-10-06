using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TehGM.Discord.Interactions.CommandsHandling.Registration.Services;

namespace TehGM.Discord.Interactions.CommandsHandling.Registration.Tests
{
    [TestFixture]
    [Category("Services")]
    public class DiscordInteractionCommandBuilderTests
    {
        private IDiscordInteractionCommandBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<SingletonService>();
            services.AddTransient<TransientService>();
            this._builder = new DiscordInteractionCommandBuilder(services.BuildServiceProvider());
        }

        [Test]
        public async Task CommandBuilder_BuildsCommand_FromAttribute()
        {
            Type handlerType = typeof(MockHandler_Attribute);
            CancellationToken cancellationToken = default;

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("name-attribute", result.Name);
        }

        [Test]
        public async Task CommandBuilder_BuildsCommand_FromSyncBuilder()
        {
            Type handlerType = typeof(MockHandler_Builder_Sync);
            CancellationToken cancellationToken = default;

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("name-builder", result.Name);
        }

        [Test]
        public async Task CommandBuilder_BuildsCommand_FromAsyncBuilder()
        {
            Type handlerType = typeof(MockHandler_Builder_Async);
            CancellationToken cancellationToken = default;

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("name-builder", result.Name);
        }

        [Test]
        public async Task CommandBuilder_MixedAttributeAndBuilder_PrefersBuilder()
        {
            Type handlerType = typeof(MockHandler_AttributeBuilderMix);
            CancellationToken cancellationToken = default;

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("name-builder", result.Name);
        }

        [Test]
        public async Task CommandBuilder_DependencyInjection_InjectsSingletonService()
        {
            Type handlerType = typeof(MockHandler_Builder_SingletonInjection);
            CancellationToken cancellationToken = default;
            Type serviceType = typeof(SingletonService);

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(serviceType.Name, result.Name);
        }

        [Test]
        public async Task CommandBuilder_DependencyInjection_InjectsTransientService()
        {
            Type handlerType = typeof(MockHandler_Builder_TransientInjection);
            CancellationToken cancellationToken = default;
            Type serviceType = typeof(TransientService);

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(serviceType.Name, result.Name);
        }

        [Test]
        public async Task CommandBuilder_DependencyInjection_InjectsCancellationToken()
        {
            Type handlerType = typeof(MockHandler_Builder_CancellationTokenInjection);
            CancellationToken cancellationToken = default;

            DiscordApplicationCommand result = await this._builder.BuildAsync(handlerType, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(cancellationToken.GetHashCode().ToString(), result.Name);
        }

        [Test]
        public void CommandBuilder_UnmarkedHandler_Fails()
        {
            Type handlerType = typeof(MockHandler_Invalid_Unmarked);
            CancellationToken cancellationToken = default;

            TestDelegate result = () => this._builder.BuildAsync(handlerType, cancellationToken).GetAwaiter().GetResult();

            Assert.Catch(result);
        }

        [Test]
        public void CommandBuilder_MultipleBuilderMethods_Fails()
        {
            Type handlerType = typeof(MockHandler_Invalid_MultipleBuilderMethods);
            CancellationToken cancellationToken = default;

            TestDelegate result = () => this._builder.BuildAsync(handlerType, cancellationToken).GetAwaiter().GetResult();

            Assert.Catch(result);
        }

        [Test]
        public void CommandBuilder_InvalidBuilderMethodReturnType_Fails()
        {
            Type handlerType = typeof(MockHandler_Invalid_BuilderMethodWrongReturnType);
            CancellationToken cancellationToken = default;

            TestDelegate result = () => this._builder.BuildAsync(handlerType, cancellationToken).GetAwaiter().GetResult();

            Assert.Catch(result);
        }

        // MOCK SERVICES
        class SingletonService : CommandNameService { }
        class TransientService : CommandNameService { }
        abstract class CommandNameService
        {
            public string Name => GetType().Name;
        }

        // MOCK HANDLERS
        // handler marked with attribute
        [InteractionCommand("name-attribute", "description-attribute")]
        class MockHandler_Attribute : MockHandlerBase { }

        // handler with builder method, synchronous
        class MockHandler_Builder_Sync : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build() => BuilderCommmand;
        }

        // handler with builder method, asynchronous
        class MockHandler_Builder_Async : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static Task<DiscordApplicationCommand> Build() => Task.FromResult(BuilderCommmand);
        }

        // handler with both attribute and builder method
        [InteractionCommand("name-attribute", "description-attribute")]
        class MockHandler_AttributeBuilderMix : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build() => BuilderCommmand;
        }

        // handler with builder method, injecting singleton service
        class MockHandler_Builder_SingletonInjection : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build(SingletonService service) => BuilderCommandWithService(service);
        }

        // handler with builder method, injecting transient service
        class MockHandler_Builder_TransientInjection : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build(TransientService service) => BuilderCommandWithService(service);
        }

        // handler with builder method, injecting transient service
        class MockHandler_Builder_CancellationTokenInjection : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build(CancellationToken cancellationToken)
            {
                var result = BuilderCommmand;
                result.Name = cancellationToken.GetHashCode().ToString();
                return result;
            }
        }

        // handler without either attribute nor builder method
        class MockHandler_Invalid_Unmarked : MockHandlerBase { }

        // handler with more than one builder method
        class MockHandler_Invalid_MultipleBuilderMethods : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build1() => BuilderCommmand;
            [InteractionCommandBuilder]
            static DiscordApplicationCommand Build2() => BuilderCommmand;
        }

        // handler with builder method that returns invalid type
        class MockHandler_Invalid_BuilderMethodWrongReturnType : MockHandlerBase
        {
            [InteractionCommandBuilder]
            static CommandNameService Build() => null;
        }

        // base class for mocks to reduce repetition
        abstract class MockHandlerBase : IDiscordInteractionCommandHandler
        {
            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, CancellationToken cancellationToken)
                => Task.FromResult<DiscordInteractionResponse>(null);

            protected static DiscordApplicationCommand BuilderCommmand => new DiscordApplicationCommand(DiscordApplicationCommandType.ChatInput, "name-builder", "description-builder", true);
            protected static DiscordApplicationCommand BuilderCommandWithService(CommandNameService service)
            {
                var result = BuilderCommmand;
                result.Name = service.Name;
                return result;
            }
        }
    }
}
