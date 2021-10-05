using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    /// <inheritdoc/>
    public class DiscordInteractionCommandBuilder : IDiscordInteractionCommandBuilder
    {
        private readonly IServiceProvider _services;

        /// <summary>Initializes a new instance of the class.</summary>
        /// <param name="services">Services that can be used for invoking the build method.</param>
        public DiscordInteractionCommandBuilder(IServiceProvider services)
        {
            this._services = services;
        }

        /// <inheritdoc/>
        /// <summary>Builds the command by invoking method marked with <see cref="InteractionCommandBuilderAttribute"/> or using <see cref="InteractionCommandAttribute"/>.</summary>
        /// <param name="cancellationToken">Cancellation token that will be passed to the build method.</param>
        /// <param name="type">Type to look for attributes in.</param>
        public async Task<DiscordApplicationCommand> BuildAsync(Type type, CancellationToken cancellationToken)
        {
            // try to use the builder method first
            IEnumerable<MethodInfo> methods = type.GetMethods().Where(method =>
                !Attribute.IsDefined(method, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(method, typeof(InteractionCommandBuilderAttribute)));
            if (methods.Any())
            {
                // only allow one method to be marked
                if (methods.Count() > 1)
                    throw new InvalidOperationException($"Command handler {type.FullName} cannot be built - only 1 method can be marked with {nameof(InteractionCommandBuilderAttribute)}.");
                MethodInfo method = methods.First();

                // disallow non-static
                if (!method.IsStatic)
                    throw new InvalidOperationException($"Command handler {type.FullName} cannot be built - only static methods can be marked with {nameof(InteractionCommandBuilderAttribute)}.");

                // check return type
                if (!IsAllowedReturnType(method.ReturnType))
                    throw new InvalidOperationException($"Command handler {type.FullName} cannot be built - only static methods returning {nameof(DiscordApplicationCommand)} or " +
                        $"{nameof(Task<DiscordApplicationCommand>)} can be marked with {nameof(InteractionCommandBuilderAttribute)}.");

                // build params
                object[] parameterValues = BuildMethodParameters(method, this._services, cancellationToken);

                // invoke method
                object result = method.Invoke(null, parameterValues);

                // if it's a task, await it
                if (result is Task<DiscordApplicationCommand> taskResult)
                    result = await taskResult.ConfigureAwait(false);

                // if result is not a collection, wrap it into one
                if (result is DiscordApplicationCommand objectResult)
                    result = new DiscordApplicationCommand[] { objectResult };

                // finally, return the results
                return (DiscordApplicationCommand)result;
            }
            // if builder method is not there, check attribute on class
            else
            {
                InteractionCommandAttribute commandAttribute = type.GetCustomAttribute<InteractionCommandAttribute>(inherit: true);
                if (commandAttribute == null)
                    throw new InvalidOperationException($"Command handler {type.FullName} cannot be built - {nameof(InteractionCommandAttribute)} not present.");
                return new DiscordApplicationCommand(commandAttribute.CommandType, commandAttribute.Name, commandAttribute.Description, true);
            }
        }

        private static object[] _emptyParams = new object[] { };
        private static object[] BuildMethodParameters(MethodInfo method, IServiceProvider services, CancellationToken cancellationToken)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (!parameters.Any())
                return _emptyParams;

            object[] results = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo p = parameters[i];

                // try from service provider first
                object service = services.GetService(p.ParameterType);
                if (service != null)
                {
                    results[i] = service;
                    continue;
                }

                // try service provider itself
                if (p.ParameterType.IsAssignableFrom(services.GetType()))
                {
                    results[i] = services;
                    continue;
                }

                // try cancellation token
                if (p.ParameterType.IsAssignableFrom(typeof(CancellationToken)))
                {
                    results[i] = services;
                    continue;
                }

                // none found, throw
                throw new InvalidOperationException($"Unsupported param type: {p.ParameterType.FullName}");
            }

            return results;
        }

        private static bool IsAllowedReturnType(Type type)
            => typeof(DiscordApplicationCommand).IsAssignableFrom(type)
            || typeof(Task<DiscordApplicationCommand>).IsAssignableFrom(type);
    }
}
