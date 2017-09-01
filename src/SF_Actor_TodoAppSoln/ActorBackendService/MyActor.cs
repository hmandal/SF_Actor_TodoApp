// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ActorBackendService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ActorBackendService.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class MyActor : Actor, IMyActor
    {
        private const string StateName = "Todo";

        /// <summary>
        /// Initializes a new instance of ActorBackendService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public MyActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            try
            {
                bool added = await this.StateManager.TryAddStateAsync<string>(StateName, "Default Todo");

                string todo = await this.StateManager.GetStateAsync<string>(StateName);

                if (!added)
                {
                    // value already exists, which means processing has already started.
                    throw new InvalidOperationException("Processing for this actor has already started.");
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task<string> SendTodoAsync()
        {
            try
            {
                string result = await this.StateManager.AddOrUpdateStateAsync<string>(StateName, "My Todo", (k, oldVal) =>
                {
                    return oldVal.Equals("Default Todo") ? "My Todo" : "oldVal not DefaultTodo!";
                });

                string todo = await this.StateManager.GetStateAsync<string>(StateName);

                return todo;
            }
            catch (Exception)
            {
            }

            return "HM_Exception";
        }

        public async Task<string> ReceiveTodoAsync()
        {
            string todo = await this.StateManager.GetStateAsync<string>(StateName);

            ActorEventSource.Current.ActorMessage(this, $"Processing Todo. Current value: {todo}");

            return todo;
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization
            await base.OnActivateAsync();
        }
    }
}