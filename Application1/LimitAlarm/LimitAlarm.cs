using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using LimitAlarm.Interfaces;
using CommonEntity;

namespace LimitAlarm
{ 

    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class LimitAlarm : Actor, ILimitAlarm
    {
        private const string MetadataState = "metadata";
        /// <summary>
        /// Initializes a new instance of LimitAlarm
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public LimitAlarm(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            var result = await StateManager.TryGetStateAsync<LimitAlarmData>(MetadataState);
            if (!result.HasValue)
            {
                var metadata = new LimitAlarmData();
                await StateManager.TryAddStateAsync(MetadataState, metadata);
            }

        }

        public Task ProcessEventAsync(DataQualityTimestamp payload)
        {
            throw new NotImplementedException();
        }

        public async Task SetData(LimitAlarmData data)
        {
            await StateManager.SetStateAsync(MetadataState, data);
        }

        public Task<LimitAlarmData> GetData()
        {
            throw new NotImplementedException();
        }
    }
}
