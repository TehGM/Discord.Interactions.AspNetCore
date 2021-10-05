﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>Factory service that can create <see cref="IDiscordInteractionCommand"/> instances.</summary>
    public interface IDiscordInteractionCommandHandlerFactory
    {
        /// <summary>Adds a new command service descriptor for a specific command ID.</summary>
        /// <param name="commandID">ID of the command.</param>
        /// <param name="handlerDescriptor">Service descriptor describing how to initialize the handler.</param>
        void AddDescriptor(ulong commandID, ServiceDescriptor handlerDescriptor);
        /// <summary>Retrieves previously added command service descriptor using command ID.</summary>
        /// <param name="commandID">ID of the command.</param>
        /// <returns>Service descriptor describing how to create a command handler.</returns>
        ServiceDescriptor GetDescriptor(ulong commandID);
        /// <summary>Creates an instance of a command handler from the descriptor.</summary>
        /// <param name="handlerDescriptor">Descriptor to create the command from.</param>
        /// <param name="services">Service provider to use for creating the handler isntance.</param>
        /// <returns>Created handler instance.</returns>
        IDiscordInteractionCommand CreateCommand(ServiceDescriptor handlerDescriptor, IServiceProvider services);
        /// <summary>Removes a specific cached command handler descriptor from the factory.</summary>
        /// <param name="commandID">ID of the command.</param>
        /// <returns>True if the descriptor was found and removed; otherwise false.</returns>
        bool RemoveDescriptor(ulong commandID);
        /// <summary>Removes all handler descriptors cached with this factory.</summary>
        void Clear();
    }
}
