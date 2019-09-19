using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using StoreHotData.Interfaces;
using CommonEntity;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace StoreHotData
{
    
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Volatile)]
    internal class StoreHotData : Actor, IStoreHotData
    {
        IDatabase cache;
        /// <summary>
        /// Initializes a new instance of StoreHotData
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public StoreHotData(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }       

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            cache = RedisConnectorHelper.Connection.GetDatabase();

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        public async Task StoreData(string key, DataQualityTimestamp value)
        {
            var strValue = JsonConvert.SerializeObject(value);
            await cache.StringSetAsync(key, strValue);
        }
    }

    internal class RedisConnectorHelper
    {
        static RedisConnectorHelper()
        {
            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("localhost");
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
