using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Thing.Interfaces;
using CommonEntity;
using Attribute.Interfaces;

namespace Thing
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
    internal class Thing : Actor, IThing
    {
        private const string MetadataState = "metadata";


        private readonly Dictionary<string, IAttribute> actorProxyDictionary = new Dictionary<string, IAttribute>();
        /// <summary>
        /// Initializes a new instance of Thing
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Thing(ActorService actorService, ActorId actorId) 
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
            var result = await StateManager.TryGetStateAsync<ThingMetaData>(MetadataState);
            if (!result.HasValue)
            {
                var metadata = new ThingMetaData()
                {
                    ID = 1,
                    Name = ""
                };                
                await StateManager.TryAddStateAsync(MetadataState, metadata);
            }
        }

        public async Task<ThingData> GetData()
        {
            ThingData metadata = new ThingData()
            {
                AttributeDatas = new List<AttributeData>()
            };

            var metadataResult = await StateManager.TryGetStateAsync<ThingMetaData>(MetadataState);
            if (metadataResult.HasValue)
            {
                metadata.ThingMetaData = metadataResult.Value;
            }
            else
            {
                metadata.ThingMetaData = new ThingMetaData
                                        {
                                            Name = "",
                                            ID = -1
                                        };
            }

            foreach (var attribute in actorProxyDictionary)
            {
                var attributeData = await attribute.Value.GetData();
                metadata.AttributeDatas.Add(attributeData);
            }
            return metadata;
        }

        public async Task<bool> ProcessEventAsync(IEnumerable<AttributeRuntimeData> attributeRuntimeDatas)
        {
            try
            {
                if (attributeRuntimeDatas == null)
                {
                    return false;
                }

                var eventDataList = attributeRuntimeDatas as IList<AttributeRuntimeData> ?? attributeRuntimeDatas.ToList();

                foreach (AttributeRuntimeData payload in eventDataList)
                {
                    // Invoke Actor
                    if (payload == null)
                    {
                        continue;
                    }

                    // Invoke Device Actor
                    var proxy = GetAttributeProxy(Id + payload.Name);
                    if (proxy != null)
                    {
                        await proxy.ProcessEventAsync(payload.dqt);
                    }
                }
            }           
            catch (AggregateException ex)
            {
                // Trace Exception
                foreach (var exception in ex.InnerExceptions)
                {
                    ActorEventSource.Current.Message(exception.Message);
                }
                return false;
            }
            catch (Exception ex)
            {
                // Trace Exception
                ActorEventSource.Current.Message(ex.Message);
                return false;
            }

            return true;
        }

        public async Task SetData(ThingData data)
        {
            // Validate parameter
            if (data == null)
            {
                return;
            }

            await StateManager.SetStateAsync(MetadataState, data.ThingMetaData);

            foreach (var attributeData in data.AttributeDatas)
            {
                var attribute = GetAttributeProxy(Id +attributeData.AttributeMetaData.Name);
                await attribute.SetData(attributeData);
            }
        }

        private IAttribute GetAttributeProxy(string attributeID)
        {
            lock (actorProxyDictionary)
            {
                if (actorProxyDictionary.ContainsKey(attributeID))
                {
                    return actorProxyDictionary[attributeID];
                }
                actorProxyDictionary[attributeID] = ActorProxy.Create<IAttribute>(new ActorId(attributeID), new Uri("fabric:/Application1/AttributeActorService"));
                return actorProxyDictionary[attributeID];
            }
        }
    }
}
